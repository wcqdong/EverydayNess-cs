using Common.Utils;
using ConnService.Netty;
using ConnService.Netty.Kcp;
using Core;
using Core.Attributes;
using Core.Config;
using Core.Utils;

namespace ConnService;

public class StartUp
{
    [Starter]
    public static void OnStartUp()
    {
        // TODO Init Socket Config
        string s = CoreConst.BaseDir;
        KcpConfig.Inst = YamlUtils.Load<KcpConfig>($"{CoreConst.BaseDir}../Config/Netty.yml");

        // TODO Start Socket Server
        KcpServer server = new KcpServer();
        server.Start();;

        NetStat.Start();
    }
}