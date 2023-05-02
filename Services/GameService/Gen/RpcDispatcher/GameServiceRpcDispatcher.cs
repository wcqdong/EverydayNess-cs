using Core.Attributes;
using Core.Core;

namespace GameService.Gen.RpcDispatcher;

[Generated]
public class GameServiceRpcDispatcher : ServiceRpcDispatcherBase
{
    public override EReturnType GetReturnType(int methodKey)
    {
        switch (methodKey)
        {
            case 2:
                return EReturnType.OBJECT;
            default:
                throw new Exception("GameServiceProxyDispatcher::Call 没找到rpc函数");
        }
    }

    public override void CallVoid(Service service, int methodKey, object[] methodParams)
    {
        throw new NotImplementedException();
    }

    public override object CallObject(Service service, int methodKey, object[] methodParams)
    {
        GameService serv = (service as GameService)!;
        switch (methodKey)
        {
            case 2:
                return serv.Test2((int)methodParams[0], (string)methodParams[1]);
            default:
                throw new Exception("GameServiceProxyDispatcher::Call 没找到rpc函数");
        }
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