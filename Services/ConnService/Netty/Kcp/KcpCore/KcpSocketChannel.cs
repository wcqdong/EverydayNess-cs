using System.Net;
using System.Runtime.InteropServices;
using DotNetty.Buffers;
using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace ConnService.Netty.Kcp.KcpCore;

public class KcpSocketChannel : AbstractChannel
{

    static readonly ChannelMetadata ChannelMetadata = new ChannelMetadata(true);

    public KcpAgent Kcp { get; }

    public KcpOutput _kcpOutput;

    private readonly KcpState _state;

    private uint _nextUpdateTick;

    public new EndPoint RemoteAddress { get; set; }

    private readonly IInternalLogger _logger = InternalLoggerFactory.GetInstance<KcpSocketChannel>();


    public uint Conv { get; }

    public override IChannelConfiguration Configuration { get; }
    public KcpOptions KcpOptions => ((KcpSocketChannelConfiguration)Configuration).KcpOptions;

    private new KcpServerSocketChannel Parent { get; }

    public KcpSocketChannel(KcpServerSocketChannel parent, uint conv, EndPoint remoteAddress) : base(parent)
    {
        Parent = parent;
        Conv = conv;
        RemoteAddress = remoteAddress;

        _kcpOutput = KcpOutput;
        // 创建kcp
        Kcp = new KcpAgent(Conv, _kcpOutput);
        // 创建Option配置
        Configuration = new KcpSocketChannelConfiguration(this);

        // 连接状态
        _state = new KcpState(this);
        // 初始化连接状态
        _state.Init();
    }


    #region AbstractChannel Override

    protected override void DoRegister()
    {
        base.DoRegister();

        _state.OnRegister();
    }

    protected override void DoWrite(ChannelOutboundBuffer input)
    {
        var sent = false;
        if (input.Size == 0)
        {
            return;
        }

        if (!_state.CanWrite())
        {
            _logger.Debug($"kcp state:{_state}, channelId:{Id}");
            return;
        }

        var waitSnd = Kcp.WaitSnd;
        if (waitSnd > KcpOptions.SndWnd * 2)
        {
            _logger.Warn($"kcp wait send size too large.close socket, channel id:{Id} conv:{Conv} waitSnd:{waitSnd}");

            _ = SafeClose();

            return;
        }

        while (true)
        {
            object msg = input.Current;
            if (msg == null)
            {
                // Wrote all messages.
                break;
            }

            if (msg is not IByteBuffer buf)
            {
                Pipeline.FireExceptionCaught(new InvalidCastException("kcp socket channel:invalid message type."));
                return;
            }

            byte flag = buf.ReadByte();
            switch (flag)
            {
                case KcpConst.Udp:
                {
                    // _logger.Info($"发送UDP数据包:\n{ByteBufferUtil.PrettyHexDump(content)}");
                    // udp传输
                    Parent.WriteAndFlushAsync(new DatagramPacket(buf.RetainedSlice(), RemoteAddress)).ContinueWith(t =>
                    {
                        if (t.Exception != null)
                        {
                            _logger.Error($"发送UDP数据包失败:{t.Exception}");
                        }
                    });
                    break;
                }
                default:
                {
                    // kcp传输
                    var code = KcpWrite(buf);
                    if (code < 0)
                    {
                        // 抛出异常
                        Pipeline.FireExceptionCaught(new Exception(
                            $"kcp send failed.code:{code} data length:{buf.ReadableBytes}"));
                    }

                    break;
                }

            }

            sent = true;

            input.Remove();
        }

        if (sent)
        {
            // kcp send 之后下一帧立即更新
            _nextUpdateTick = 0;
        }
    }

    private int KcpWrite(IByteBuffer buffer)
    {
        var readableBytes = buffer.ReadableBytes;
        // TODO 有必要在这分段吗，kcp里会分段
        // TODO 导致问题：1.多次c++调用 2.上次的lastBytes，这次一整个SegmentSize，kcp内还是会分segment
        var segmentCount = readableBytes / KcpOptions.SegmentSize;
        var lastBytes = readableBytes % KcpOptions.SegmentSize;

        for (var i = 0; i < segmentCount; i++)
        {
            var code = Kcp.Send(buffer.GetIoBuffer(buffer.ReaderIndex, KcpOptions.SegmentSize));
            if (code < 0)
            {
                return code;
            }

            buffer.SetReaderIndex(buffer.ReaderIndex + KcpOptions.SegmentSize);
        }

        if (lastBytes > 0)
        {
            var code = Kcp.Send(buffer.GetIoBuffer(buffer.ReaderIndex, lastBytes));
            if (code < 0)
            {
                return code;
            }

            buffer.SetReaderIndex(buffer.ReaderIndex + lastBytes);
        }

        return 0;
    }

    private async Task SafeClose()
    {
        try
        {
            await CloseAsync();
        }
        catch (Exception e)
        {
            _logger.Error($"Kcp socket channel close error.channel id:{Id} conv:{Conv} exception:{e}");
        }
    }

    public override bool Open => _state.IsOpen();
    public override bool Active => _state.IsActive();
    public override ChannelMetadata Metadata => ChannelMetadata;
    protected override EndPoint LocalAddressInternal => Parent.LocalAddress;
    protected override EndPoint RemoteAddressInternal => RemoteAddress;

    protected override IChannelUnsafe NewUnsafe() => new KcpSocketChannelUnsafe(this);

    protected override bool IsCompatible(IEventLoop eventLoop) => true;

    private sealed class KcpSocketChannelUnsafe : AbstractUnsafe
    {
        public KcpSocketChannelUnsafe(AbstractChannel channel)
            : base(channel)
        {
        }

        public override Task ConnectAsync(EndPoint remoteAddress, EndPoint localAddress)
        {
            throw new NotSupportedException();
        }
    }

    protected override void DoBind(EndPoint localAddress)
    {
        throw new NotSupportedException();
    }

    protected override void DoDisconnect()
    {
        _state.OnDisconnect();
    }
    protected override void DoDeregister()
    {
        Kcp.Dispose();
    }

    protected override void DoClose()
    {
        _state.OnClose();
        Parent.CloseChildChannel(Conv);
    }

    protected override void DoBeginRead()
    {
        // read operation be control by parent
    }

    #endregion


    #region Kcp
    public void KcpInput(IByteBuffer data, EndPoint remoteAddress)
    {
        var now = DateTime.UtcNow;
        try
        {
            if (Kcp.Disposed)
                return;

            //
            if (Kcp.WaitSnd > KcpOptions.SndWnd)
            {
                _logger.Warn($"wait send window:{Kcp.WaitSnd}");
            }

            if (Kcp.Input(data.GetIoBuffer()) < 0)
            {
                _logger.Warn($"kcp input error");
                return;
            }

            // 更新地址
            if (!RemoteAddress.Equals(remoteAddress))
            {
                RemoteAddress = remoteAddress;
            }

            while (true)
            {
                var size = Kcp.PeekSize();
                if (size <= 0)
                    break;

                var buffer = Allocator.Buffer(size);

                // 获取写缓冲区
                var readBytes = Kcp.Recv(buffer.GetIoBuffer(buffer.WriterIndex, size));
                // 设置写索引
                buffer.SetWriterIndex(readBytes);

                Pipeline.FireChannelRead(new DatagramMessage(true, buffer));
            }

            _state.LastRecvTime = now;
        }
        finally
        {
            data.Release();
        }
    }

    private int KcpOutput(IntPtr buf, int len, IntPtr kcp, IntPtr user)
    {
        var buffer = Allocator.DirectBuffer(len);

        var writeBuffer = buffer.GetIoBuffer(buffer.WriterIndex, len);

        Marshal.Copy(buf, writeBuffer.Array, writeBuffer.Offset, len);
        buffer.SetWriterIndex(buffer.WriterIndex + len);

        Parent.WriteAndFlushAsync(new DatagramPacket(buffer, RemoteAddress));

        return len;
    }

    public void KcpUpdate(uint tick)
    {
        if (Kcp.Disposed)
        {
            return;
        }

        if (tick >= _nextUpdateTick)
        {
            Kcp.Update(tick);

            _nextUpdateTick = Kcp.Check(tick);
        }
    }


    #endregion


    public void SetKcpOptions(KcpOptions kcpOptions)
    {
        Kcp.NoDelay(kcpOptions.NoDelay, kcpOptions.Interval, kcpOptions.ReSend, kcpOptions.Nc);
        Kcp.SetMtu(kcpOptions.Mtu);
        Kcp.SetStream(kcpOptions.StreamMode);
        Kcp.WndSize(kcpOptions.SndWnd, kcpOptions.RcvWnd);
    }
}