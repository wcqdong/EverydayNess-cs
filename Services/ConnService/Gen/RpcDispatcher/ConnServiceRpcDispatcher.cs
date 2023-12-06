using Core.Core;

namespace ConnService.Gen.RpcDispatcher;

public class ConnServiceRpcDispatcher : ServiceRpcDispatcherBase
{
    public override EReturnType GetReturnType(int methodKey)
    {
        switch (methodKey)
        {
            default:
                throw new Exception($"ConnServiceProxyDispatcher::GetReturnType 没找到methodKey={methodKey}的rpc函数");
        }

    }

    public override void CallVoid(Service service, int methodKey, object[] methodParams)
    {
       ConnService serv = (service as ConnService)!;
        switch (methodKey)
        {
            default:
                throw new Exception($"ConnServiceProxyDispatcher::CallVoid 没找到methodKey={methodKey}的rpc函数");
        }

    }

    public override object CallObject(Service service, int methodKey, object[] methodParams)
    {
         ConnService serv = (service as ConnService)!;
         switch (methodKey)
         {
             default:
                 throw new Exception($"ConnServiceProxyDispatcher::CallObject 没找到methodKey={methodKey}的rpc函数");
         }
    }

    public override async Task CallAsyncVoid(Service service, int methodKey, object[] methodParams)
    {
         ConnService serv = (service as ConnService)!;
         switch (methodKey)
         {
             default:
                 throw new Exception($"ConnServiceProxyDispatcher::CallTaskVoid 没找到methodKey={methodKey}的rpc函数");
         }
    }

    public override async Task<object> CallAsyncObject(Service service, int methodKey, object[] methodParams)
    {
        ConnService serv = (service as ConnService)!;
        switch (methodKey)
        {
            default:
                throw new Exception($"ConnServiceProxyDispatcher::CallTaskObject 没找到methodKey={methodKey}的rpc函数");
        }
    }
}

