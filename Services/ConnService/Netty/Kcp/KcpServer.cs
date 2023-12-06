using System.Net;
using ConnService.Netty.Kcp.KcpCore;
using DotNetty.Transport.Channels;

namespace ConnService.Netty.Kcp;

public class KcpServer : SocketServer
{
    public void Start()
    {
        _ = RunKcpServer();
    }

    public static int KcpOutput(IntPtr buf, int len, IntPtr kcp, IntPtr user)
    {

        return 1;
    }

    private async Task RunKcpServer()
    {
        KcpServerBootstrap bootstrap = new KcpServerBootstrap();

        MultithreadEventLoopGroup group = new MultithreadEventLoopGroup(KcpConfig.Ports.Count);
        bootstrap.Group(group)
            .ChannelFactory(() => new KcpServerSocketChannel())
            .Option(ChannelOption.SoReuseaddr, true)
            .Option(ChannelOption.SoBacklog, 2048)
            .ChildOption(KcpChannelOption.KcpOptions, new KcpOptions())
            ;

        // handler
        bootstrap.ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
        {
            channel.Pipeline.AddLast("KcpHandler", new KcpHandler());
        }));

        foreach (var port in KcpConfig.Ports)
        {
            IChannel channel = await bootstrap.BindAsync(IPAddress.Parse(KcpConfig.Host), port);
            Console.WriteLine($"Netty Bind Port {port}");
        }
    }
}