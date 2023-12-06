using Core.Config;
using Core.Core;

namespace Common.Gen.Proxy;

public class ConnServiceProxy
{
    public static ConnServiceProxy Inst(CallPoint callPoint)
    {
        return new ConnServiceProxy(callPoint);
    }

    public static ConnServiceProxy Inst(string nodeIdId, string portId, object serviceId)
    {
        return new ConnServiceProxy(nodeIdId, portId, serviceId);
    }


    public CallPoint CallPoint { get; }

    public ConnServiceProxy(string nodeIdId, string portId, object serviceId)
    {
        CallPoint = new CallPoint(nodeIdId, portId, serviceId);
    }
    public ConnServiceProxy(CallPoint callPoint)
    {
        CallPoint = callPoint;
    }


    #region RPC

    #endregion
}
