namespace Core.Core;

public abstract class ProxyDispatcherBase
{
    public enum EReturnType
    {
        VOID,
        OBJECT,
        TASK_VOID,
        TASK_OBJECT
    }
    // public abstract void Call(Service service, int methodKey, object[] methodParams);
    //
    // public abstract object CallReturn(Service service, int methodKey, object[] methodParams);

    // public abstract void Call(Service service, int methodKey, object[] methodParams);
    //
    // public abstract object CallReturn(Service service, int methodKey, object[] methodParams);
    //
    //
    // public abstract Task Call(Service service, int methodKey, object[] methodParams);
    //
    // public abstract object CallReturn(Service service, int methodKey, object[] methodParams);

    public abstract EReturnType GetReturnType(int methodKey);

    public abstract void CallVoid(Service service, int methodKey, object[] methodParams);
    public abstract object CallObject(Service service, int methodKey, object[] methodParams);

    public abstract Task CallTaskVoid(Service service, int methodKey, object[] methodParams);

    public abstract Task<object> CallTaskObject(Service service, int methodKey, object[] methodParams);


}