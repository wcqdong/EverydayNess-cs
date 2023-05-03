using Core.Core;

namespace MatchService.Gen.RpcDispatcher;

public class MatchServiceRpcDispatcher : ServiceRpcDispatcherBase
{
    public override EReturnType GetReturnType(int methodKey)
    {
        switch (methodKey)
        {
            case 1:
                return EReturnType.OBJECT;
            default:
                throw new Exception($"MatchServiceProxyDispatcher::GetReturnType 没找到methodKey={methodKey}的rpc函数");
        }

    }

    public override void CallVoid(Service service, int methodKey, object[] methodParams)
    {
       MatchService serv = (service as MatchService)!;
        switch (methodKey)
        {
            default:
                throw new Exception($"MatchServiceProxyDispatcher::CallVoid 没找到methodKey={methodKey}的rpc函数");
        }

    }

    public override object CallObject(Service service, int methodKey, object[] methodParams)
    {
         MatchService serv = (service as MatchService)!;
         switch (methodKey)
         {
             case 1:
                 return serv.Test4((int)methodParams[0]);
             default:
                 throw new Exception($"MatchServiceProxyDispatcher::CallObject 没找到methodKey={methodKey}的rpc函数");
         }
    }

    public override async Task CallAsyncVoid(Service service, int methodKey, object[] methodParams)
    {
         MatchService serv = (service as MatchService)!;
         switch (methodKey)
         {
             default:
                 throw new Exception($"MatchServiceProxyDispatcher::CallTaskVoid 没找到methodKey={methodKey}的rpc函数");
         }
    }

    public override async Task<object> CallAsyncObject(Service service, int methodKey, object[] methodParams)
    {
        MatchService serv = (service as MatchService)!;
        switch (methodKey)
        {
            default:
                throw new Exception($"MatchServiceProxyDispatcher::CallTaskObject 没找到methodKey={methodKey}的rpc函数");
        }
    }
}

