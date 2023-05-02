using Core.Extensions;
using Core.Utils;

namespace Core.Core;

public abstract class Service
{
    private Port _port;
    public Port Port
    {
        get => _port;
        set
        {
            _port = value;
            _port.AddTask(_init);
        }
    }

    public string ServiceId { get; }

    /// <summary>
    /// rpc分发
    /// </summary>
    public ServiceRpcDispatcherBase ServiceRpcDispatcher { get; set; }


    public Service(string serviceId)
    {
        ServiceId = serviceId;
    }

    public virtual void Tick(long now)
    {

        // scheduler.AddTask(new Task(Tick));
    }

    protected virtual void Init()
    {
    }

    private void _init()
    {
        string fullName = CoreUtils.ServiceToFullName(ServiceId);
        string dispatcherName = $"{fullName}.Gen.RpcDispatcher.{fullName}RpcDispatcher";
        Type? type = GetType().Assembly.GetType(dispatcherName);
        if (type == null)
        {
            throw new Exception($"{dispatcherName}没生成，请通过工具生成");
        }
        ServiceRpcDispatcher = (ServiceRpcDispatcherBase)Activator.CreateInstance(type)!;
        Init();
    }
}