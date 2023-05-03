namespace Core.Core;

public abstract class ServiceRpcDispatcherBase
{
    public abstract EReturnType GetReturnType(int methodKey);

    public abstract void CallVoid(Service service, int methodKey, object[] methodParams);
    public abstract object CallObject(Service service, int methodKey, object[] methodParams);

    public abstract Task CallAsyncVoid(Service service, int methodKey, object[] methodParams);

    public abstract Task<object> CallAsyncObject(Service service, int methodKey, object[] methodParams);


}

public enum EReturnType
{
    VOID = 1,
    OBJECT = 2,
    ASYNC_VOID = 3,
    ASYNC_OBJECT = 4
}