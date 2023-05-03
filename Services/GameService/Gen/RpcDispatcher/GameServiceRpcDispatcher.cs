using Core.Core;

namespace GameService.Gen.RpcDispatcher;

public class GameServiceRpcDispatcher : ServiceRpcDispatcherBase
{
    public override EReturnType GetReturnType(int methodKey)
    {
        switch (methodKey)
        {
            case 1:
                return EReturnType.VOID;
            case 2:
                return EReturnType.OBJECT;
            case 3:
                return EReturnType.OBJECT;
            case 4:
                return EReturnType.ASYNC_OBJECT;
            default:
                throw new Exception($"GameServiceProxyDispatcher::GetReturnType 没找到methodKey={methodKey}的rpc函数");
        }

    }

    public override void CallVoid(Service service, int methodKey, object[] methodParams)
    {
       GameService serv = (service as GameService)!;
        switch (methodKey)
        {
            case 1:
                serv.Test1();
                return;
            default:
                throw new Exception($"GameServiceProxyDispatcher::CallVoid 没找到methodKey={methodKey}的rpc函数");
        }

    }

    public override object CallObject(Service service, int methodKey, object[] methodParams)
    {
         GameService serv = (service as GameService)!;
         switch (methodKey)
         {
             case 2:
                 return serv.Test2((int)methodParams[0], (string)methodParams[1]);
             case 3:
                 return serv.Test3((int)methodParams[0], (string)methodParams[1]);
             default:
                 throw new Exception($"GameServiceProxyDispatcher::CallObject 没找到methodKey={methodKey}的rpc函数");
         }
    }

    public override async Task CallAsyncVoid(Service service, int methodKey, object[] methodParams)
    {
         GameService serv = (service as GameService)!;
         switch (methodKey)
         {
             default:
                 throw new Exception($"GameServiceProxyDispatcher::CallTaskVoid 没找到methodKey={methodKey}的rpc函数");
         }
    }

    public override async Task<object> CallAsyncObject(Service service, int methodKey, object[] methodParams)
    {
        GameService serv = (service as GameService)!;
        switch (methodKey)
        {
            case 4:
                return await serv.Test4((int)methodParams[0], (string)methodParams[1]);
            default:
                throw new Exception($"GameServiceProxyDispatcher::CallTaskObject 没找到methodKey={methodKey}的rpc函数");
        }
    }
}

