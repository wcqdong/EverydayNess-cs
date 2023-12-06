using ConnService.Netty;
using ConnService.Netty.Kcp;
using Core.Attributes;

namespace ConnService;

public class StartUp
{
    [Starter]
    public static void OnStartUp()
    {

        // TODO Init Socket Config

        // TODO Start Socket Server
        KcpServer server = new KcpServer();
        server.Start();;

        NetStat.Start();
    }
}