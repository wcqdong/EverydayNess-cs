using ConnService.Netty.Kcp.KcpCore;

namespace ConnService.Netty.Kcp;

/// <summary>
/// TODO 从yml中读取
/// </summary>
public class KcpConfig : KcpOptions
{
    public static string Host = "0.0.0.0";
    // public static string Host = "127.0.0.1";

    public static List<int> Ports = new()
    {
        7500,
        7501,
        7502,
        7503,
        7504,
        7505,
        // 7506,
        // 7507,
        // 7508,
        // 7509,
        // 7510,
        // 7511,
        // 7512,
        // 7513,
        // 7514,
        // 7515,
    };
}