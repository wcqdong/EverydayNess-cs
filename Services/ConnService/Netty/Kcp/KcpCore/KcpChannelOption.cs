using DotNetty.Transport.Channels;

namespace ConnService.Netty.Kcp.KcpCore;

public class KcpChannelOption
{
    public static ChannelOption<KcpOptions> KcpOptions = ChannelOption.ValueOf<KcpOptions>("KCP_OPTION");
    public static ChannelOption<KcpOptions> Kcp_Keeplive = ChannelOption.ValueOf<KcpOptions>("KCP_KEEPLIVE");
}

public class KcpOptions
{
    public int UpdateTime = 10;

    public int Mtu = 1400;


    public int RcvWnd = 512;
    public int SndWnd = 512;

    /** Kcp 报文头大小 */
    public int KcpOverHeadSize = 24;

    /** Kcp 最小接收窗口 */
    public int KcpMinRcvWndSize = 128;

    public int SegmentSize;


    public int NoDelay { get; } = 1;
    // interval ：协议内部工作的 interval，单位毫秒，比如 10ms或者 20ms
    public int Interval { get; } = 10;
    // resend ：快速重传模式，默认0关闭，可以设置2（2次ACK跨越将会直接重传）
    public int ReSend { get; } = 2;
    // nc ：是否关闭流控，默认是0代表不关闭，1代表关闭。
    public int Nc { get; } = 1;

    public bool StreamMode { get; } = false;

    public KcpOptions()
    {
        SegmentSize = (Mtu - KcpOverHeadSize) * (KcpMinRcvWndSize - 1);
    }

}

public class KcpSocketChannelConfiguration : DefaultChannelConfiguration
{
    private KcpSocketChannel KcpSocketChannel => (KcpSocketChannel)Channel;

    public KcpOptions KcpOptions = new();

    public KcpSocketChannelConfiguration(IChannel channel) : base(channel)
    {
    }

    public KcpSocketChannelConfiguration(IChannel channel, IRecvByteBufAllocator allocator) : base(channel, allocator)
    {
    }

    public override bool SetOption<T>(ChannelOption<T> option, T value)
    {
        if (KcpChannelOption.KcpOptions.Equals(option))
        {
            if (value is KcpOptions kcpOptions)
            {
                KcpOptions = kcpOptions;
                KcpSocketChannel.SetKcpOptions(kcpOptions);
            }
        }
        return base.SetOption(option, value);
    }

    public override T GetOption<T>(ChannelOption<T> option)
    {
        return base.GetOption(option);
    }
}