using DotNetty.Transport.Channels;

namespace NettyClient;

public class KcpClientConfig
{
    public static readonly int ConnectNum = 1000;
    public static readonly List<IChannel> ConnectChannels = new();
    public static string Host = "127.0.0.1";
    // public static string Host = "10.0.3.37";
    // public static string Host = "10.42.34.107";
    // public static readonly string Host = "10.0.7.7";
    // public static readonly int[] Ports = {
    //     7001, 7002, 7003, 7004,
    //     7005, 7006, 7007, 7008,
    //     7009, 7010, 7011, 7012,
    //     7013, 7014, 7015, 7016,
    // };

    public static readonly int PortNum = 8;
    public static readonly int LoopNum = 8;
    public static readonly int SendNum = 20;       // 每秒发送多少个包
    public static readonly string SendData =
        "Hello world, The program sets up a 256 bit key and a 128 bit IV. This is appropriate for the 256-bit AES encryption that we going to be doing in CBC mode. " +
        "Hello world, The program sets up a 256 bit key and a 128 bit IV. This is appropriate for the 256-bit AES encryption that we going to be doing in";

    public static byte[] KcpData = Array.Empty<byte>();
}