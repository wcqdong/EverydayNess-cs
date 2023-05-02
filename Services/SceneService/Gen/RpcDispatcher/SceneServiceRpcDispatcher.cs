using Core.Attributes;
using Core.Core;

namespace SceneService.Gen.RpcDispatcher;

[Generated]
public class SceneServiceRpcDispatcher : ServiceRpcDispatcherBase
{
    public override EReturnType GetReturnType(int methodKey)
    {
        switch (methodKey)
        {
            case 1:
                return EReturnType.TASK_OBJECT;
            case 3:
                return EReturnType.VOID;
            case 11:
                return EReturnType.VOID;
            case 22:
                return EReturnType.OBJECT;
            case 33:
                return EReturnType.TASK_VOID;
            case 44:
                return EReturnType.TASK_OBJECT;
            default:
                throw new Exception("SceneServiceProxyDispatcher::Call 没找到rpc函数");
        }
    }

    public override void CallVoid(Service service, int methodKey, object[] methodParams)
    {
        SceneService serv = (service as SceneService)!;
        switch (methodKey)
        {
            case 3:
                serv.Test3((int)methodParams[0], (string)methodParams[1]);
                break;
            case 11:
                serv.t1((int)methodParams[0], (string)methodParams[1]);
                break;
            default:
                throw new Exception("SceneServiceProxyDispatcher::Call 没找到rpc函数");
        }
    }

    public override object CallObject(Service service, int methodKey, object[] methodParams)
    {
        SceneService serv = (service as SceneService)!;
        switch (methodKey)
        {
            case 22:
                return serv.t2((int)methodParams[0], (string)methodParams[1]);
            default:
                throw new Exception("SceneServiceProxyDispatcher::Call 没找到rpc函数");
        }
    }

    public override async Task CallTaskVoid(Service service, int methodKey, object[] methodParams)
    {
        SceneService serv = (service as SceneService)!;
        switch (methodKey)
        {
            case 22:
                await serv.t3((int)methodParams[0], (string)methodParams[1]);
                return;
            default:
                throw new Exception("SceneServiceProxyDispatcher::Call 没找到rpc函数");
        }
    }

    public override async Task<object> CallTaskObject(Service service, int methodKey, object[] methodParams)
    {
        SceneService serv = (service as SceneService)!;
        switch (methodKey)
        {
            case 1:
                return await serv.Test1((int)methodParams[0], (string)methodParams[1]);
            case 22:
                return await serv.t4((int)methodParams[0], (string)methodParams[1]);
            default:
                throw new Exception("SceneServiceProxyDispatcher::Call 没找到rpc函数");
        }
    }
}