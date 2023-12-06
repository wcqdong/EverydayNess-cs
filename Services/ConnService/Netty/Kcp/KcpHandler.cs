using ConnService.Netty.Kcp.KcpCore;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace ConnService.Netty.Kcp;

public class KcpHandler : ChannelHandlerAdapter
{

    public override void ChannelRead(IChannelHandlerContext context, object message)
    {
        Analyzer.Receive();

        DatagramMessage msg = (DatagramMessage)message;

        Analyzer.ReceiveLength((ulong)msg.Buffer.ReadableBytes);

        // 写回客户端
        context.Channel.WriteAsync(msg.Buffer);
    }
}