using System.Net;
using System.Net.Sockets;
using DotNetty.Buffers;
using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace ConnService.Netty.Kcp.KcpCore;

public class KcpServerSocketChannel : SocketDatagramChannel, IServerChannel
{
    private static uint _conv;

    private readonly Dictionary<uint, KcpSocketChannel> _childChannels = new ();

    private readonly IInternalLogger _logger = InternalLoggerFactory.GetInstance<KcpServerSocketChannel>();

    public KcpServerSocketChannel() : base(new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
    {
    }

    private uint ApplyConv()
    {
        while (true)
        {
            var conv = Interlocked.Increment(ref _conv);
            var b = conv & 0x000000FF;
            if (b != KcpConst.Udp && b != KcpConst.Syn && b != KcpConst.Ack)
                return conv;
        }
    }

    #region SocketDatagramChannel Override

    protected override void DoRegister()
    {
        base.DoRegister();

        // EventLoop.Schedule(Pulse, KcpOptions.UpdateTime);
    }

    protected override int DoReadMessages(List<object> buf)
    {
        SocketChannelAsyncOperation readOperation = ReadOperation;
        IByteBuffer data = (IByteBuffer) readOperation.UserToken!;
        bool free = true;
        try
        {
            int bytesTransferred = readOperation.BytesTransferred;
            if (bytesTransferred <= 0)
                return 0;

            Unsafe.RecvBufAllocHandle.LastBytesRead = bytesTransferred;
            data.SetWriterIndex(data.WriterIndex + bytesTransferred);
            EndPoint remoteAddress = readOperation.RemoteEndPoint!;

            byte signature = data.GetByte(data.ReaderIndex);
            switch (signature)
            {
                case KcpConst.Udp:
                {
                    // UDP包
                    data.SkipBytes(1);
                    // 读取kcp会话id
                    var conv = data.ReadUnsignedIntLE();
                    if (_childChannels.TryGetValue(conv, out var channel))
                    {
                        var payload = data.Slice();
                        free = false;

                        channel.Pipeline.FireChannelRead(new DatagramMessage(false, payload));
                    }

                    break;
                }
                case KcpConst.Syn:
                {
                    // 连接包
                    var child = new KcpSocketChannel(this, ApplyConv(), remoteAddress);
                    _childChannels.TryAdd(child.Conv, child);
                    // 初始化channel
                    buf.Add(child);
                    break;
                }
                default:
                {
                    // kcp数据包
                    var connId = data.GetUnsignedIntLE(data.ReaderIndex);
                    if (_childChannels.TryGetValue(connId, out var child))
                    {
                        child.KcpInput(data, remoteAddress);
                        free = false;
                    }

                    break;
                }
            }

            return 1;
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
            _logger.Error(e);
            return 0;
        }
        finally
        {
            if (free)
                data.Release();
            readOperation.UserToken = null;
        }
    }

    #endregion

    #region KCP

    public void Tick()
    {
        if (!Active)
            return;
        var iNow = KcpUtils.GetTick();
        KeepLive(iNow);
        Update(iNow);
    }

    private void KeepLive(uint inow)
    {
        // foreach (var child in _channels)
        // {
        // }
    }

    private void Update(uint inow)
    {
        foreach (var child in _childChannels)
        {
            try
            {
                child.Value.KcpUpdate(inow);
            }
            catch (Exception e)
            {
                _logger.Error($"{nameof(KcpSocketChannel)} update error:{e}");
            }
        }
    }

    #endregion

    public void CloseChildChannel(uint conv)
    {
        _childChannels.Remove(conv, out _);
    }




}