using Core.Config;
using Core.Core;

namespace Common.Gen.Proxy;

public class MatchServiceProxy
{
    private static MatchServiceProxy Instance;
    private static readonly object _lock = new();

    public static MatchServiceProxy Inst
    {
        get
        {
            if (Instance == null)
            {
                lock (_lock)
                {
                    CallPoint callPoint = DistributeConfig.Inst.GlobalCallPoints["match"];
                    Instance ??= new MatchServiceProxy(callPoint);
                }
            }

            return Instance;
        }
    }


    public CallPoint CallPoint { get; }

    public MatchServiceProxy(string nodeIdId, string portId, object serviceId)
    {
        CallPoint = new CallPoint(nodeIdId, portId, serviceId);
    }
    public MatchServiceProxy(CallPoint callPoint)
    {
        CallPoint = callPoint;
    }


    #region RPC

    public async Task<string> Test4(int a)
    {
       Port port = Port.GetCurrent();
       Call call = port.MakeCall(CallPoint, 1, a);
       return await port.SendCall<string>(call);
    }

    #endregion
}
