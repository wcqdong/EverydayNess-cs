using Core.Core;

namespace SceneService.Gen.RpcDispatcher;

public class SceneServiceRpcDispatcher : ServiceRpcDispatcherBase
{
    public override EReturnType GetReturnType(int methodKey)
    {
        switch (methodKey)
        {
            case 1:
                return EReturnType.ASYNC_OBJECT;
            case 2:
                return EReturnType.VOID;
            case 3:
                return EReturnType.VOID;
            case 4:
                return EReturnType.OBJECT;
            case 5:
                return EReturnType.ASYNC_VOID;
            case 6:
                return EReturnType.ASYNC_OBJECT;
            default:
                throw new Exception($"SceneServiceProxyDispatcher::GetReturnType 没找到methodKey={methodKey}的rpc函数");
        }

    }

    public override void CallVoid(Service service, int methodKey, object[] methodParams)
    {
       SceneService serv = (service as SceneService)!;
        switch (methodKey)
        {
            case 2:
                serv.Test3((int)methodParams[0], (string)methodParams[1]);
                return;
            case 3:
                serv.t1((int)methodParams[0], (string)methodParams[1]);
                return;
            default:
                throw new Exception($"SceneServiceProxyDispatcher::CallVoid 没找到methodKey={methodKey}的rpc函数");
        }

    }

    public override object CallObject(Service service, int methodKey, object[] methodParams)
    {
         SceneService serv = (service as SceneService)!;
         switch (methodKey)
         {
             case 4:
                 return serv.t2((int)methodParams[0], (string)methodParams[1]);
             default:
                 throw new Exception($"SceneServiceProxyDispatcher::CallObject 没找到methodKey={methodKey}的rpc函数");
         }
    }

    public override async Task CallAsyncVoid(Service service, int methodKey, object[] methodParams)
    {
         SceneService serv = (service as SceneService)!;
         switch (methodKey)
         {
             case 5:
                await serv.t3((int)methodParams[0], (string)methodParams[1]);
                 return;
             default:
                 throw new Exception($"SceneServiceProxyDispatcher::CallTaskVoid 没找到methodKey={methodKey}的rpc函数");
         }
    }

    public override async Task<object> CallAsyncObject(Service service, int methodKey, object[] methodParams)
    {
        SceneService serv = (service as SceneService)!;
        switch (methodKey)
        {
            case 1:
                return await serv.Test1((int)methodParams[0], (string)methodParams[1]);
            case 6:
                return await serv.t4((int)methodParams[0], (string)methodParams[1]);
            default:
                throw new Exception($"SceneServiceProxyDispatcher::CallTaskObject 没找到methodKey={methodKey}的rpc函数");
        }
    }
}

