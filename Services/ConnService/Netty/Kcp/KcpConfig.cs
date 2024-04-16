using ConnService.Netty.Kcp.KcpCore;

namespace ConnService.Netty.Kcp;

/// <summary>
/// </summary>
public class KcpConfig : KcpOptions
{
    public static KcpConfig Inst = null!;

    public string Host  { get; set; }

    public List<int> Ports  { get; set; } = new();

    // public KcpConfig(string host)
    // {
    //     Host = host;
    // }
}