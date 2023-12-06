using ConnService.Netty.Kcp.KcpCore;
using DotNetty.Transport.Channels;

namespace ConnService.Netty.Kcp;

public class KcpHandler : ChannelHandlerAdapter
{

    public override void ChannelRead(IChannelHandlerContext context, object message)
    {
        NetStat.IncReceiveCount(1);

        DatagramMessage msg = (DatagramMessage)message;

        NetStat.IncReceiveBytes(msg.Buffer.ReadableBytes);

        // 写回客户端
        context.Channel.WriteAsync(msg.Buffer);

        NetStat.IncSendCount(1);
        NetStat.IncSendBytes(msg.Buffer.ReadableBytes);
    }
}