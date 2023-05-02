using Core.Core;

namespace MatchService.Gen.RpcDispatcher;

public class MatchServiceRpcDispatcher : ServiceRpcDispatcherBase
{
    public override EReturnType GetReturnType(int methodKey)
    {
        throw new NotImplementedException();
    }

    public override void CallVoid(Service service, int methodKey, object[] methodParams)
    {
        throw new NotImplementedException();
    }

    public override object CallObject(Service service, int methodKey, object[] methodParams)
    {
        throw new NotImplementedException();
    }

    public override Task CallTaskVoid(Service service, int methodKey, object[] methodParams)
    {
        throw new NotImplementedException();
    }

    public override Task<object> CallTaskObject(Service service, int methodKey, object[] methodParams)
    {
        throw new NotImplementedException();
    }
}