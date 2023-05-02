using System.Collections.Concurrent;
using System.Diagnostics;
using ConsoleApp63;
using Core.Support;
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


    private uint callbackId;

    private Dictionary<uint, CallBack> _callBacks = new();

    private ConcurrentQueue<Call> _calls = new ();

    private List<Call> _affirmCalls = new ();

    private ConcurrentQueue<Call> _callResults = new ();

    private List<Call> _affirmCallResults = new ();

    private long _now;

    // private Call? _during;

    private Dictionary<object, Service> _services = new();

    private readonly ConcurrentQueue<Action> tasks = new();


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
        Affirm();
        _watch.Stop();
        Console.WriteLine($"{PortId},Affirm,{_watch.ElapsedMilliseconds}");


        // 处理calls
        _watch.Restart();
        TickCalls();
        _watch.Stop();
        Console.WriteLine($"{PortId},TickCalls,{_watch.ElapsedMilliseconds}");

        // 处理超时的回调
        _watch.Restart();
        TickCallBackTimeout();
        _watch.Stop();
        Console.WriteLine($"{PortId},TickCallBackTimeout,{_watch.ElapsedMilliseconds}");

        // Tick驱动service
        _watch.Restart();
        TickServices();
        _watch.Stop();
        Console.WriteLine($"{PortId},TickServices,{_watch.ElapsedMilliseconds}");

        // Tick等待任务队列
        _watch.Restart();
        TickTasks();
        _watch.Stop();
        Console.WriteLine($"{PortId},TickTasks,{_watch.ElapsedMilliseconds}");

        _watch.Stop();


    }

    private void TickCallBackTimeout()
    {
        List<uint> removeKeys = _callBacks.Where((kv) => _now > kv.Value.timeout).Select((kv) => kv.Key).ToList();
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

    private void TickTasks()
    {
        while (tasks.TryDequeue(out var task))
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
                doCall(call);
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

    private async void doCall(Call call)
    {
        try
        {
            if(!_services.TryGetValue(call.To.ServiceId, out Service? service))
            {
                Console.WriteLine($"doCall {call.To.ServiceId} 不存在");
                return;
            }

            ServiceRpcDispatcherBase.EReturnType returnType = service.ServiceRpcDispatcher.GetReturnType(call.MethodKey);
            switch (returnType)
            {
                case ServiceRpcDispatcherBase.EReturnType.VOID:
                    service.ServiceRpcDispatcher.CallVoid(service, call.MethodKey, call.MethodParams);
                    break;
                case ServiceRpcDispatcherBase.EReturnType.OBJECT:
                    call.result = service.ServiceRpcDispatcher.CallObject(service, call.MethodKey, call.MethodParams);
                    Returns(call);
                    break;
                case ServiceRpcDispatcherBase.EReturnType.TASK_VOID:
                    await service.ServiceRpcDispatcher.CallTaskVoid(service, call.MethodKey, call.MethodParams);
                    break;
                case ServiceRpcDispatcherBase.EReturnType.TASK_OBJECT:
                    call.result = await service.ServiceRpcDispatcher.CallTaskObject(service, call.MethodKey, call.MethodParams);
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

        AddCall(call);
    }

    /// <summary>
    /// 确认在此帧要执行的call和callResult
    /// </summary>
    private void Affirm()
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


    public Task<T> AddCall<T>(Call call)
    {
        _scheduler.Call = call;

        Task<T> task = Task.Factory.StartNew(() => (T)_scheduler.Call.result, CancellationToken.None, TaskCreationOptions.None,  _scheduler);
        return task;
    }

    public void AddCall(Call call)
    {
        Dispatch(call);
    }

    public void AddCallback(Call call, Task task)
    {
        // 需要回调则设置callbackId
        call.CallBackId = callbackId++;
        _callBacks.Add(call.CallBackId, new CallBack(call, task));
    }

    public void Dispatch(Call call)
    {
        // // 需要回调则设置callbackId
        // if (call.NeedResult)
        // {
        //     call.CallBackId = callbackId++;
        //     _callBacks.Add(call.CallBackId, new CallBack(call, task));
        // }
        // // 不需要返回，则直接让await代码继续执行
        // else
        // {
        //     _scheduler.CallBack(task);
        // }

        // 同进程
        if (Node.NodeId.Equals(call.To.NodeId))
        {
            Node.Dispatch(call);
        }
        else
        {
            // TODO 通过TCP或其他通信方式发送出去
        }
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
