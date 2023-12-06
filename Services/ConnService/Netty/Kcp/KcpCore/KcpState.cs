using System.Reflection.Metadata;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace ConnService.Netty.Kcp.KcpCore;

/// <summary>
/// TODO 连接管理
/// </summary>
public class KcpState
{
    private KcpSocketChannel _socketChannel;
    /// <summary>
    /// 是否维护连接状态
    /// </summary>
    public bool KeepAlive { get; set; } = false;
    /// <summary>
    /// 上次收到消息的时间
    /// </summary>
    public DateTime LastRecvTime { get; set; }


    public KcpState(KcpSocketChannel socketChannel)
    {
        _socketChannel = socketChannel;
    }
    public bool CanWrite()
    {
        return true;
    }

    public bool IsClose()
    {
        return false;
    }

    public void Init()
    {

    }

    public bool IsOpen()
    {
        return true;
    }

    public bool IsActive()
    {
        return true;
    }

    public void OnDisconnect()
    {
        throw new NotImplementedException();
    }

    public void OnClose()
    {
        throw new NotImplementedException();
    }

    public void OnRegister()
    {
        //conv的值不同, 不然可以用一块固定内存
        var buffer = _socketChannel.Allocator.Buffer(sizeof(byte) + sizeof(uint));
        buffer.WriteByte(KcpConst.Syn);
        buffer.WriteIntLE(unchecked((int)_socketChannel.Conv));
        _socketChannel.Parent.WriteAndFlushAsync(new DatagramPacket(buffer, _socketChannel.RemoteAddress));
    }
}