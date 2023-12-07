using ConnService.Netty.Kcp.KcpCore;
using DotNetty.Transport.Channels;

namespace ConnService.Netty.Kcp;

public class KcpHandler : ChannelHandlerAdapter
{

    public override void ChannelRead(IChannelHandlerContext context, object message)
    {
        DatagramMessage msg = (DatagramMessage)message;

        int bytes = msg.Buffer.ReadableBytes;
        NetStat.IncReceiveCount(1);
        NetStat.IncReceiveBytes(bytes);

        // 写回客户端
        context.Channel.WriteAndFlushAsync(msg.Buffer);

        NetStat.IncSendCount(1);
        NetStat.IncSendBytes(bytes);
    }
}