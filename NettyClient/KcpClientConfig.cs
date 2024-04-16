using ConnService.Netty.Kcp;
using DotNetty.Transport.Channels;

namespace NettyClient;

public class KcpClientConfig
{
    public static KcpClientConfig Inst = null!;


    public string Host  { get; set; }

    public List<int> Ports  { get; set; } = new();

    public int ConnectNum { get; set; }

    public int SendPerSecond  { get; set; }      // 每秒发送多少个包
    public string SendData { get; set; }

    public int LoopNum { get; set; }

    public bool NetStat { get; set; }

    public int SendDataLengh { get; set; }

    public List<IChannel> ConnectChannels = new();

}