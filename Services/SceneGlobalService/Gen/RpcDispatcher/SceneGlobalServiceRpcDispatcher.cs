using Core.Core;

namespace SceneGlobalService.Gen.RpcDispatcher;

public class SceneGlobalServiceRpcDispatcher : ServiceRpcDispatcherBase
{
    public override EReturnType GetReturnType(int methodKey)
    {
        switch (methodKey)
        {
            default:
                throw new Exception($"SceneGlobalServiceProxyDispatcher::GetReturnType 没找到methodKey={methodKey}的rpc函数");
        }

    }

    public override void CallVoid(Service service, int methodKey, object[] methodParams)
    {
       SceneGlobalService serv = (service as SceneGlobalService)!;
        switch (methodKey)
        {
            default:
                throw new Exception($"SceneGlobalServiceProxyDispatcher::CallVoid 没找到methodKey={methodKey}的rpc函数");
        }

    }

    public override object CallObject(Service service, int methodKey, object[] methodParams)
    {
         SceneGlobalService serv = (service as SceneGlobalService)!;
         switch (methodKey)
         {
             default:
                 throw new Exception($"SceneGlobalServiceProxyDispatcher::CallObject 没找到methodKey={methodKey}的rpc函数");
         }
    }

    public override async Task CallAsyncVoid(Service service, int methodKey, object[] methodParams)
    {
         SceneGlobalService serv = (service as SceneGlobalService)!;
         switch (methodKey)
         {
             default:
                 throw new Exception($"SceneGlobalServiceProxyDispatcher::CallTaskVoid 没找到methodKey={methodKey}的rpc函数");
         }
    }

    public override async Task<object> CallAsyncObject(Service service, int methodKey, object[] methodParams)
    {
        SceneGlobalService serv = (service as SceneGlobalService)!;
        switch (methodKey)
        {
            default:
                throw new Exception($"SceneGlobalServiceProxyDispatcher::CallTaskObject 没找到methodKey={methodKey}的rpc函数");
        }
    }
}

