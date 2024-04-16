using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using ConnService.Netty.Kcp;
using ConnService.Netty.Kcp.KcpCore;
using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Primitives;

namespace NettyClient;

public class KcpClientHandler : SimpleChannelInboundHandler<DatagramPacket>
{
    private KcpClientChannel kcpChannel;

    public KcpClientHandler(): base(true)
    {
        var port = KcpClientConfig.Inst.Ports[Random.Shared.Next(KcpClientConfig.Inst.Ports.Count)];
        kcpChannel = new KcpClientChannel(new IPEndPoint(IPAddress.Parse(KcpClientConfig.Inst.Host), port));
    }

    [SuppressMessage("ReSharper.DPA", "DPA0000: DPA issues")]
    private IByteBuffer GetData()
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        int random = Random.Shared.Next(1000);

        StringBuilder sbf = new StringBuilder();
        sbf.Append(now);
        sbf.Append(random);
        sbf.Append(KcpClientConfig.Inst.SendData);

        int randomDataLen = Random.Shared.Next(1, KcpClientConfig.Inst.SendDataLengh);
        for (int i = 0; i < randomDataLen; i++)
        {
            sbf.Append(KcpClientConfig.Inst.SendData);
        }

        var bytes = Encoding.UTF8.GetBytes(sbf.ToString());
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

        Console.WriteLine($"{kcpChannel.RemoteAddr} connecting……");

    }

    [SuppressMessage("ReSharper.DPA", "DPA0000: DPA issues")]
    private void SendPacket()
    {
        if (!kcpChannel.IsConnect())
        {
            return;
        }
        var num = KcpClientConfig.Inst.SendPerSecond / 2;
        for (var i = 0; i < num; ++i)
        {
            IByteBuffer buffer = GetData();

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
            if (syn != KcpConst.Ack)
            {
                return;
            }
            uint conv = (uint)msg.Content.ReadIntLE();
            kcpChannel.Connect(conv);

            Console.WriteLine($"[{conv}] 连接成功");

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