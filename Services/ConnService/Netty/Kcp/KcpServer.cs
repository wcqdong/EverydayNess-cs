using System.Net;
using ConnService.Netty.Kcp.KcpCore;
using DotNetty.Transport.Channels;

namespace ConnService.Netty.Kcp;

public class KcpServer : SocketServer
{
    public void Start()
    {
        RunKcpServer().Wait();
    }

    private async Task RunKcpServer()
    {
        KcpOptions kcpOptions = new KcpOptions();

        KcpServerBootstrap bootstrap = new KcpServerBootstrap(kcpOptions.UpdateTime);

        MultithreadEventLoopGroup group = new MultithreadEventLoopGroup(KcpConfig.Ports.Count);
        bootstrap.Group(group)
            .ChannelFactory(() => new KcpServerSocketChannel())
            .Option(ChannelOption.SoReuseaddr, true)
            .Option(ChannelOption.SoBacklog, 2048)
            .ChildOption(KcpChannelOption.KcpOptions, kcpOptions)
            ;

        // handler
        bootstrap.ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
        {
            channel.Pipeline.AddLast("KcpHandler", new KcpHandler());
        }));

        foreach (var port in KcpConfig.Ports)
        {
            await bootstrap.BindAsync(IPAddress.Parse(KcpConfig.Host), port);
            Console.WriteLine($"Listen on {KcpConfig.Host}:{port}");
        }

    }
}