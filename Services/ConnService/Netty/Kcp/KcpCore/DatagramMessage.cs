using DotNetty.Buffers;

namespace ConnService.Netty.Kcp.KcpCore;

public class DatagramMessage
{
    /// <summary>
    /// 是否可靠
    /// </summary>
    public bool IsReliable;

    /// <summary>
    /// 消息数据
    /// </summary>
    public IByteBuffer Buffer;

    public DatagramMessage(bool isReliable, IByteBuffer buffer)
    {
        IsReliable = isReliable;
        Buffer = buffer;
    }
}