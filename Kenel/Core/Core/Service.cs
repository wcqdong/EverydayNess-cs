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
            _port.AddTask(Init);
        }
    }

    public string ServiceId { get; }

    public ProxyDispatcherBase ProxyDispatcher;



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



        // scheduler.AddTask(new Task(Tick));
    }
}