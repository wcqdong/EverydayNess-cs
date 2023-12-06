namespace ConnService.Netty.Kcp.KcpCore;

public class KcpConst
{
    // UDP数据包标志
    public const byte Udp = 0xEC;
    // 连接标志
    public const byte Syn = 0xED;
    // 确认连接标记
    public const byte Ack = 0xEE;
    // udp标志位长度
    public const int FlagLength = sizeof(byte);

    public enum StateFlags
    {
        Open = 1,
        HalfOpen = 1 << 1,
        ReadScheduled = 1 << 2,
        WriteScheduled = 1 << 3,
        Active = 1 << 4,
        HalfClosed = 1 << 5,
        // todo: add input shutdown and read pending here as well?
    }
}