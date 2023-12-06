using System.Net;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace NettyClient;

public class TcpClient
{
    private static int _connectNum = 800;
    private static List<IChannel> _connectChannels = new();
    // private static string _host = "127.0.0.1";
    private static string _host = "10.0.3.37";
    // private static string _host = "10.42.34.107";
    private static int[] _ports = new[]
    {
        7516, 7501, 7502, 7503,
        7504, 7505, 7506, 7507,
        7508, 7509, 7510, 7511,
        7512, 7513, 7514, 7515,
    };
    private static int _portNum = 16;
    // private static int _loopNum = 16;
    private static int _sendNum = 20;       // 每秒发送多少个包
    private static string _sendData =
        "Hello world, The program sets up a 256 bit key and a 128 bit IV. This is appropriate for the 256-bit AES encryption that we going to be doing in CBC mode. " +
        "Hello world, The program sets up a 256 bit key and a 128 bit IV. This is appropria te for the 256-bit AES encryption that we going to be doing in";

    static async Task RunClientAsync()
    {
        var group = new MultithreadEventLoopGroup();
        try
        {
            var bootstrap = new Bootstrap();
            bootstrap
                .Group(group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                    pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));
                    pipeline.AddLast("echo", new EchoClientHandler());
                }));


            for (var i = 0; i < _connectNum; ++i)
            {
                var port = _ports[Random.Shared.Next(_portNum)];
                var removeAddr = new IPEndPoint(IPAddress.Parse(_host), port);
                IChannel clientChannel = await bootstrap.ConnectAsync(removeAddr);
                _connectChannels.Add(clientChannel);
            }

            Console.ReadLine();

            foreach (var channel in _connectChannels)
            {
                await channel.CloseAsync();
            }
        }
        finally
        {
            await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
        }
    }

    public static void Start()
    {
        NetStat.Start();
        RunClientAsync().Wait();
    }

    public class EchoClientHandler : ChannelHandlerAdapter
    {
        private IChannel _channel;
        
        private IByteBuffer GetData()
        {
            var bytes = Encoding.UTF8.GetBytes(_sendData);
            var message = Unpooled.Buffer(bytes.Length);
            message.WriteBytes(bytes);
            return message;
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            _channel = context.Channel;
            context.Channel.EventLoop.Schedule(OnSendPacket, TimeSpan.FromMilliseconds(Random.Shared.Next(100, 5000)));
        }
        
        private void OnSendPacket()
        {
            var num = _sendNum / 2;
            for (var i = 0; i < num; ++i)
            {
                var buffer = GetData();
                NetStat.IncSendCount(1);
                NetStat.IncSendBytes(buffer.ReadableBytes);
                
                _channel.WriteAsync(buffer);   
            }
            _channel.Flush();
            
            _channel.EventLoop.Schedule(OnSendPacket, TimeSpan.FromMilliseconds(500));
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            if (message is IByteBuffer buffer)
            {
                NetStat.IncReceiveCount(1);
                NetStat.IncReceiveBytes(buffer.ReadableBytes);
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("Exception: " + exception);
            context.CloseAsync();
        }
    }
    
}