using Core.Support;

namespace Core.Core;

public class CallBack
{
    public Call Call;
    public Task callBack;

    // 用来超时检查
    public long timeout;

    public CallBack(Call call, Task task)
    {
        Call = call;
        callBack = task;
        timeout = Port.GetTime() + CoreConst.CallbackTimeout;
    }
}