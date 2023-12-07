using System.Net;
using System.Runtime.InteropServices;
using ConnService;
using ConnService.Netty;
using ConnService.Netty.Kcp.KcpCore;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace NettyClient;

public class KcpClientChannel
{

    public uint Conv;

    public IPEndPoint RemoteAddr;

    private readonly KcpOutput _kcpOutput;

    public IChannel Channel { get; set; }

    private KcpAgent _kcp;

    public KcpOptions KcpOptions= new ();

    public KcpClientChannel(IPEndPoint ipEndPoint)
    {
        RemoteAddr = ipEndPoint;

        _kcpOutput = KcpOutput;
    }

    private int KcpOutput(IntPtr buf, int len, IntPtr kcp, IntPtr user)
    {
        var buffer = Channel.Allocator.DirectBuffer(len);

        var writeBuffer = buffer.GetIoBuffer(buffer.WriterIndex, len);

        Marshal.Copy(buf, writeBuffer.Array, writeBuffer.Offset, len);
        buffer.SetWriterIndex(buffer.WriterIndex + len);

        Channel.WriteAndFlushAsync(new DatagramPacket(buffer, RemoteAddr));

        return len;
    }

    public void WriteAndFlushAsync(IByteBuffer buffer)
    {
        Channel.WriteAndFlushAsync(new DatagramPacket(buffer, RemoteAddr));
    }

    public bool IsConnect()
    {
        return Conv > 0;
    }

    public void Send(ArraySegment<byte> buffer)
    {
        NetStat.IncSendCount(1);
        NetStat.IncSendBytes(buffer.Count);

        _kcp.Send(buffer);
    }

    public void Schedule(Action packet, TimeSpan tp)
    {
        Channel.EventLoop.Schedule(packet, tp);
    }

    public void Connect(uint conv)
    {
        Conv = conv;
        _kcp = new KcpAgent(conv, _kcpOutput);

        _kcp.NoDelay(KcpOptions.NoDelay, KcpOptions.Interval, KcpOptions.ReSend, KcpOptions.Nc);
        _kcp.SetMtu(KcpOptions.Mtu);
        _kcp.SetStream(KcpOptions.StreamMode);
        _kcp.WndSize(KcpOptions.SndWnd, KcpOptions.RcvWnd);

    }

    public void Receive(DatagramPacket msg)
    {
        if (_kcp.Input(msg.Content.GetIoBuffer()) < 0)
        {
            return;
        }

        while (true)
        {
            var size = _kcp.PeekSize();
            if (size <= 0)
                break;

            var buffer = Channel.Allocator.Buffer(size);

            // 获取写缓冲区
            var readBytes = _kcp.Recv(buffer.GetIoBuffer(buffer.WriterIndex, size));
            // 设置写索引
            buffer.SetWriterIndex(readBytes);

            NetStat.IncReceiveCount(1);
            NetStat.IncReceiveBytes(readBytes);
        }
    }

    public void Update()
    {
        uint tick = KcpUtils.GetTick();
        _kcp.Update(tick);
    }
}