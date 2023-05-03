// #define PRINT

using System.Collections.Concurrent;
using System.Diagnostics;
using ConsoleApp63;
using Core.Utils;

namespace Core.Core;

public class Port : IThread
{
    private static ThreadLocal<Port> LocalPort = new();

    public Node Node { get; set; }
    public string PortId { get; }

    public ThreadHandler _thread;

    private PortTaskScheduler _scheduler;

    private Stopwatch _watch = new();


    private int callbackId;

    private Dictionary<int, CallBack> _callBacks = new();

    private ConcurrentQueue<Call> _calls = new ();

    private List<Call> _affirmCalls = new ();

    private ConcurrentQueue<Call> _callResults = new ();

    private List<Call> _affirmCallResults = new ();

    private long _now;

    private Dictionary<object, Service> _services = new();

    private readonly ConcurrentQueue<Action> tasks = new();
    private readonly List<Action> _affirmedTasks = new();

    private readonly Dictionary<string, CallFrameBuffer> _callFrameBuffers = new();


    /// <summary>
    /// 当前运行在Main线程
    /// </summary>
    /// <param name="node"></param>
    public Port(string portId)
    {
        PortId = portId;

        _scheduler = new PortTaskScheduler(this);

        // 创建线程，但不启动，因为此时系统启动还没完成，在StartUp中启动
        _thread = new ThreadHandler(this);
    }

    /// <summary>
    /// 当前运行在Main线程
    /// </summary>
    /// <param name="node"></param>
    public void StartUp(Node node)
    {
        Node = node;
        Node.AddPort(this);

        _thread.StartUp();
    }

    /// <summary>
    /// Port所在线程启动时调用
    /// 当前线程为_thread线程
    /// </summary>
    public void OnStart()
    {
        CoreUtils.WriteLine($"{PortId} 线程启动");

        LocalPort.Value = this;

        var context = new PortSynchronizationContext();
        SynchronizationContext.SetSynchronizationContext(context);
    }

    public void OnTick()
    {
        // 获得当前帧的时间
        _now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // 确认这一帧要处理的call和returnCall
        _watch.Restart();
        AffirmCall();
        _watch.Stop();
#if PRINT
        Console.WriteLine($"{PortId},AffirmCall,{_watch.ElapsedMilliseconds}");
#endif

        // 处理calls
        _watch.Restart();
        TickCalls();
        _watch.Stop();
#if PRINT
        Console.WriteLine($"{PortId},TickCalls,{_watch.ElapsedMilliseconds}");
#endif

        // 处理超时的回调
        _watch.Restart();
        TickCallBackTimeout();
        _watch.Stop();
#if PRINT
        Console.WriteLine($"{PortId},TickCallBackTimeout,{_watch.ElapsedMilliseconds}");
#endif

        // Tick驱动service
        _watch.Restart();
        TickServices();
        _watch.Stop();
#if PRINT
        Console.WriteLine($"{PortId},TickServices,{_watch.ElapsedMilliseconds}");
#endif

        // Tick等待任务队列
        _watch.Restart();
        AffirmTask();
        _watch.Stop();
#if PRINT
        Console.WriteLine($"{PortId},AffirmTask,{_watch.ElapsedMilliseconds}");
#endif

        _watch.Restart();
        TickTask();
        _watch.Stop();
#if PRINT
        Console.WriteLine($"{PortId},TickTasks,{_watch.ElapsedMilliseconds}");
#endif

        _watch.Restart();
        FlushCallFrameBuffer();
        _watch.Stop();
#if PRINT
        Console.WriteLine($"{PortId},FlushCallFrameBuffer,{_watch.ElapsedMilliseconds}");
#endif
    }

    private void FlushCallFrameBuffer()
    {
        foreach (var item in _callFrameBuffers)
        {
            item.Value.Flush(item.Key, Node);
        }
    }

    private void TickCallBackTimeout()
    {
        List<int> removeKeys = _callBacks.Where((kv) => _now > kv.Value.timeout).Select((kv) => kv.Key).ToList();
        foreach (var removeKey in removeKeys)
        {
            CallBack callBack = _callBacks[removeKey];
            _callBacks.Remove(removeKey);
            CoreUtils.WriteLine($"{callBack.Call.FromPort} 调用 {callBack.Call.To} 超时,callbackId={removeKey}");
        }
        // foreach (var item in _callBacks)
        // {
        //     if()
        // }
    }

    private void AffirmTask()
    {
        // while (tasks.TryDequeue(out var task))
        // {
        //     try
        //     {
        //         task();
        //     }
        //     catch (Exception e)
        //     {
        //         Console.WriteLine(e);
        //     }
        // }

        while (tasks.TryDequeue(out var task))
        {
            _affirmedTasks.Add(task);
        }
    }

    private void TickTask()
    {
        foreach (var task in _affirmedTasks)
        {
            try
            {
                task();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        _affirmedTasks.Clear();
    }

    private void TickServices()
    {
        foreach (var service in _services.Values)
        {
            service.Tick(_now);
        }
    }

    public void OnStop()
    {

    }

    public static Port GetCurrent()
    {
        return LocalPort.Value;
    }

    private void TickCalls()
    {
        foreach (var call in _affirmCalls)
        {
            if (call.Type == 0)
            {
                DoCall(call);
            }
            else
            {
                doCallResult(call);
            }
        }
        // 清空
        _affirmCalls.Clear();
    }

    private void doCallResult(Call call)
    {
        try
        {
            if(!_callBacks.TryGetValue(call.CallBackId, out var callback))
            {
                CoreUtils.WriteLine($"回调不存在 callbackId = {call.CallBackId}");
                return;
            }

            // 移除回调
            _callBacks.Remove(call.CallBackId);

            // 设置返回参数
            _scheduler.Call = call;
            // 执行回调
            _scheduler.CallBack(callback.callBack);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

    }

    private async void DoCall(Call call)
    {
        try
        {
            if(!_services.TryGetValue(call.To.ServiceId, out Service? service))
            {
                Console.WriteLine($"doCall {call.To.ServiceId} 不存在");
                return;
            }

            EReturnType returnType = service.ServiceRpcDispatcher.GetReturnType(call.MethodKey);
            switch (returnType)
            {
                case EReturnType.VOID:
                    service.ServiceRpcDispatcher.CallVoid(service, call.MethodKey, call.MethodParams);
                    break;
                case EReturnType.OBJECT:
                    call.result = service.ServiceRpcDispatcher.CallObject(service, call.MethodKey, call.MethodParams);
                    Returns(call);
                    break;
                case EReturnType.ASYNC_VOID:
                    await service.ServiceRpcDispatcher.CallAsyncVoid(service, call.MethodKey, call.MethodParams);
                    break;
                case EReturnType.ASYNC_OBJECT:
                    call.result = await service.ServiceRpcDispatcher.CallAsyncObject(service, call.MethodKey, call.MethodParams);
                    Returns(call);
                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            // _during = null;
        }
    }

    private void Returns(Call sourceCall)
    {
        Call call = new Call
        {
            Type = 1,
            FromNode = sourceCall.To.NodeId,
            FromPort = sourceCall.To.PortId,
            To = new CallPoint
            {
                NodeId = sourceCall.FromNode,
                PortId = sourceCall.FromPort
            },
            CallBackId = sourceCall.CallBackId,
            result = sourceCall.result,
        };

        SendCall(call);
    }

    /// <summary>
    /// 确认在此帧要执行的call和callResult
    /// </summary>
    private void AffirmCall()
    {
        while(_calls.TryDequeue(out var call))
        {
            _affirmCalls.Add(call);
        }

        while(_callResults.TryDequeue(out var callResult))
        {
            _affirmCallResults.Add(callResult);
        }
    }


    public Task<T> SendCall<T>(Call call)
    {
        _scheduler.Call = call;

        Task<T> task = Task.Factory.StartNew(() => (T)_scheduler.Call.result, CancellationToken.None, TaskCreationOptions.None,  _scheduler);
        return task;
    }

    public void SendCall(Call call)
    {
        if (!_callFrameBuffers.TryGetValue(call.To.NodeId, out var buffer))
        {
            buffer = new CallFrameBuffer();
            _callFrameBuffers.Add(call.To.NodeId, buffer);
        }

        if (!buffer.WriteCall(call))
        {
            // TODO 长度检查，防止超过缓冲区
        }
    }

    public Call MakeCall(CallPoint callPoint, int methodId, params object[] parameters)
    {
        Call call = new Call
        {
            FromNode = Node.NodeId,
            FromPort = PortId,
            To = callPoint,
            MethodKey = methodId,
            MethodParams = parameters
        };

        return call;
    }

    public void AddCallback(Call call, Task task)
    {
        // 需要回调则设置callbackId
        call.CallBackId = callbackId++;
        _callBacks.Add(call.CallBackId, new CallBack(call, task));
    }

    public void AddQueue(Call call)
    {
        _calls.Enqueue(call);
    }

    public void AddService(Service service)
    {
        _services.Add(service.ServiceId, service);
        service.Port = this;
    }

    public void Run()
    {
        throw new NotImplementedException();
    }

    public void AddTask(Action task)
    {
        tasks.Enqueue(task);
    }

    public static long GetTime()
    {
        return Port.GetCurrent()._now;
    }
}
