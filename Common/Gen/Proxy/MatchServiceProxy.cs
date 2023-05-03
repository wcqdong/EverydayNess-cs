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

    private MatchServiceProxy(string nodeIdId, string portId, object serviceId)
    {
        CallPoint = new CallPoint(nodeIdId, portId, serviceId);
    }
    private MatchServiceProxy(CallPoint callPoint)
    {
        CallPoint = callPoint;
    }


    #region RPC

    #endregion
}
