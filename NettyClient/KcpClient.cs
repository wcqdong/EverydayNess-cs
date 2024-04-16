using System.Net;
using Common.Utils;
using ConnService.Netty;
using Core;
using Core.Utils;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace NettyClient;

public class KcpClient
{

    public static void Start()
    {
        KcpClientConfig.Inst = YamlUtils.Load<KcpClientConfig>($"{CoreConst.BaseDir}../Config/NettyClient.yml");
        if (KcpClientConfig.Inst.NetStat)
        {
            NetStat.Start();
        }
        RunClientAsync().Wait();
    }

    static async Task RunClientAsync()
    {
        var group = new MultithreadEventLoopGroup(KcpClientConfig.Inst.LoopNum);
        try
        {
            var bootstrap = new Bootstrap();
            bootstrap
                .Group(group)
                .Channel<SocketDatagramChannel>()
                .Option(ChannelOption.SoBroadcast, true)
                .Handler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast("echo", new KcpClientHandler());
                }));

            for (var i = 0; i < KcpClientConfig.Inst.ConnectNum; ++i)
            {
                IChannel clientChannel = await bootstrap.BindAsync(IPEndPoint.MinPort);
                KcpClientConfig.Inst.ConnectChannels.Add(clientChannel);
            }

            Console.ReadLine();

            foreach (var channel in KcpClientConfig.Inst.ConnectChannels)
            {
                await channel.CloseAsync();
            }
        }
        finally
        {
            await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
        }
    }


}