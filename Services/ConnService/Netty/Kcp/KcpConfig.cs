using ConnService.Netty.Kcp.KcpCore;

namespace ConnService.Netty.Kcp;

/// <summary>
/// TODO 从yml中读取
/// </summary>
public class KcpConfig : KcpOptions
{
    public static string Host = "127.0.0.1";

    public static List<int> Ports = new()
    {
        7001,
        7002,
        7003,
        7004,
        7005,
        7006,
        7007,
        7008,
    };
}