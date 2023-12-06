using System.Net;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace NettyClient;

public class KcpClient
{

    public static void Start()
    {
        NetStat.Start();
        RunClientAsync().Wait();
    }

    static async Task RunClientAsync()
    {
        var group = new MultithreadEventLoopGroup(KcpClientConfig.LoopNum);
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

            for (var i = 0; i < KcpClientConfig.ConnectNum; ++i)
            {
                IChannel clientChannel = await bootstrap.BindAsync(IPEndPoint.MinPort);
                KcpClientConfig.ConnectChannels.Add(clientChannel);
            }

            Console.ReadLine();

            foreach (var channel in KcpClientConfig.ConnectChannels)
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