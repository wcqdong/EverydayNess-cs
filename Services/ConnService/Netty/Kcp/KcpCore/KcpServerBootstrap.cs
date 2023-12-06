using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using Core.Support;
using DotNetty.Common.Internal.Logging;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;

namespace ConnService.Netty.Kcp.KcpCore;

public class KcpServerBootstrap : AbstractBootstrap<KcpServerBootstrap, IServerChannel>
{
    private static readonly IInternalLogger Logger = InternalLoggerFactory.GetInstance<ServerBootstrap>();

    private readonly ConcurrentDictionary<ChannelOption, ChannelOptionValue> _childOptions;
    private readonly ConcurrentDictionary<IConstant, AttributeValue> _childAttrs;

    private volatile IChannelHandler _childHandler;

    public KcpServerBootstrap()
    {
        _childOptions = new ConcurrentDictionary<ChannelOption, ChannelOptionValue>();
        _childAttrs = new ConcurrentDictionary<IConstant, AttributeValue>();
    }

    private KcpServerBootstrap(KcpServerBootstrap bootstrap)
        : base(bootstrap)
    {
        _childHandler = bootstrap._childHandler;
        _childOptions = new ConcurrentDictionary<ChannelOption, ChannelOptionValue>(bootstrap._childOptions);
        _childAttrs = new ConcurrentDictionary<IConstant, AttributeValue>(bootstrap._childAttrs);
    }
    
    public override KcpServerBootstrap Group(IEventLoopGroup group)
    {
        base.Group(group);

        return this;
    }

    public override KcpServerBootstrap Clone() => new KcpServerBootstrap(this);

    protected override void Init(IChannel channel)
    {
        SetChannelOptions(channel, Options, Logger);

        foreach (var e in Attributes)
        {
            e.Set(channel);
        }

        var p = channel.Pipeline;
        var channelHandler = Handler();
        if (channelHandler != null)
        {
            p.AddLast((string) null, channelHandler);
        }

        var currentChildHandler = _childHandler;
        var currentChildOptions = _childOptions.Values.ToArray();
        var currentChildAttrs = _childAttrs.Values.ToArray();

        p.AddLast(new ActionChannelInitializer<IChannel>(ch =>
        {
            ch.Pipeline.AddLast(new KcpServerBootstrapAcceptor(currentChildHandler,
                currentChildOptions, currentChildAttrs));
        }));
    }

    public KcpServerBootstrap ChildOption<T>(ChannelOption<T> childOption, T value)
    {
        Contract.Requires(childOption != null);

        if (value == null)
        {
            _childOptions.TryRemove(childOption, out _);
        }
        else
        {
            _childOptions[childOption] = new ChannelOptionValue<T>(childOption, value);
        }
        return this;
    }
    
    public KcpServerBootstrap ChildAttribute<T>(AttributeKey<T> childKey, T value)
        where T : class
    {
        Contract.Requires(childKey != null);

        if (value == null)
        {
            _childAttrs.TryRemove(childKey, out _);
        }
        else
        {
            _childAttrs[childKey] = new AttributeValue<T>(childKey, value);
        }
        return this;
    }

    public KcpServerBootstrap ChildHandler(IChannelHandler childHandler)
    {
        Contract.Requires(childHandler != null);

        _childHandler = childHandler;
        
        return this;
    }
    
    private class KcpServerBootstrapAcceptor : ChannelHandlerAdapter
    {
        private readonly IChannelHandler _childHandler;
        private readonly ChannelOptionValue[] _childOptions;
        private readonly AttributeValue[] _childAttrs;

        public KcpServerBootstrapAcceptor(IChannelHandler childHandler,
            ChannelOptionValue[] childOptions, AttributeValue[] childAttrs)
        {
            _childHandler = childHandler;
            _childOptions = childOptions;
            _childAttrs = childAttrs;
        }

        public override void ChannelRead(IChannelHandlerContext ctx, object msg)
        {
            var child = (IChannel) msg;

            child.Pipeline.AddLast((string) null, _childHandler);

            SetChannelOptions(child, _childOptions, Logger);

            foreach (AttributeValue attr in _childAttrs)
            {
                attr.Set(child);
            }

            // todo: async/await instead?
            try
            {
                ctx.Channel.EventLoop.RegisterAsync(child).ContinueWith(
                    (future, state) => ForceClose((IChannel) state, future.Exception),
                    child,
                    TaskContinuationOptions.NotOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);
            }
            catch (Exception ex)
            {
                ForceClose(child, ex);
            }
        }

        private static void ForceClose(IChannel child, Exception ex)
        {
            child.Unsafe.CloseForcibly();
            Logger.Warn("Failed to register an accepted channel: " + child, ex);
        }

        public override void ExceptionCaught(IChannelHandlerContext ctx, Exception cause)
        {
            var config = ctx.Channel.Configuration;
            if (config.AutoRead)
            {
                // stop accept new connections for 1 second to allow the channel to recover
                // See https://github.com/netty/netty/issues/1328
                config.AutoRead = false;
                ctx.Channel.EventLoop.ScheduleAsync(c => { ((IChannelConfiguration) c).AutoRead = true; }, config,
                    TimeSpan.FromSeconds(1));
            }

            // still let the ExceptionCaught event flow through the pipeline to give the user
            // a chance to do something with it
            ctx.FireExceptionCaught(cause);
        }
    }
}