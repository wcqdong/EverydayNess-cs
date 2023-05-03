namespace Core.Core;

public class PortTaskScheduler : TaskScheduler
{
    private Port _port;
    public Call? Call;

    public PortTaskScheduler(Port port)
    {
        _port = port;
    }

    protected override void QueueTask(Task task)
    {
        // 放入回调
        _port.AddCallback(Call!, task);
        // 交给线程调度
        _port.SendCall(Call!);
        // 清理Call
        Call = null;
    }

    protected override IEnumerable<Task>? GetScheduledTasks()
    {
        throw new NotImplementedException();
    }


    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    {
        throw new NotImplementedException();
    }

    public void CallBack(Task task)
    {
        TryExecuteTask(task);
    }
}