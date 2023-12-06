using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using ConnService.Netty.Kcp;
using ConnService.Netty.Kcp.KcpCore;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace NettyClient;

public class KcpClientHandler : SimpleChannelInboundHandler<DatagramPacket>
{
    private KcpClientChannel kcpChannel;

    public KcpClientHandler(): base(true)
    {
        var port = KcpConfig.Ports[Random.Shared.Next(KcpConfig.Ports.Count)];
        kcpChannel = new KcpClientChannel(new IPEndPoint(IPAddress.Parse(KcpClientConfig.Host), port));
    }

    [SuppressMessage("ReSharper.DPA", "DPA0000: DPA issues")]
    private IByteBuffer GetData()
    {
        var bytes = Encoding.UTF8.GetBytes(KcpClientConfig.SendData);
        var message = Unpooled.Buffer(bytes.Length);
        message.WriteBytes(bytes);
        return message;
    }

    public override void ChannelActive(IChannelHandlerContext context)
    {
        kcpChannel.Channel = context.Channel;

        // 发送连接请求
        var message = Unpooled.Buffer(1);
        message.WriteByte(KcpConst.Syn);

        kcpChannel.WriteAndFlushAsync(message);
    }

    [SuppressMessage("ReSharper.DPA", "DPA0000: DPA issues")]
    private void SendPacket()
    {
        if (!kcpChannel.IsConnect())
        {
            return;
        }
        var num = KcpClientConfig.SendNum / 2;
        for (var i = 0; i < num; ++i)
        {
            var buffer = GetData();

            // kcp发送
            kcpChannel.Send(buffer.GetIoBuffer());
        }

        kcpChannel.Schedule(SendPacket, TimeSpan.FromMilliseconds(500));
    }


    private void KcpUpdate()
    {
        kcpChannel.Update();

        kcpChannel.Schedule(KcpUpdate, TimeSpan.FromMilliseconds(kcpChannel.KcpOptions.Interval));
    }

    protected override void ChannelRead0(IChannelHandlerContext ctx, DatagramPacket msg)
    {
        // 刚建立连接
        if (!kcpChannel.IsConnect())
        {
            byte syn = msg.Content.ReadByte();
            if (syn != KcpConst.Syn)
            {
                return;
            }
            uint conv = (uint)msg.Content.ReadIntLE();
            kcpChannel.Connect(conv);

            kcpChannel.Schedule(SendPacket, TimeSpan.FromMilliseconds(500));
            kcpChannel.Schedule(KcpUpdate, TimeSpan.FromMilliseconds(kcpChannel.KcpOptions.Interval));
            return;
        }

        kcpChannel.Receive(msg);
    }

    public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    {
        Console.WriteLine("Exception: " + exception);
        context.CloseAsync();
    }
}