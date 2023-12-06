using Core.Config;
using Core.Core;

namespace Common.Gen.Proxy;

public class SceneServiceProxy
{
    public static SceneServiceProxy Inst(CallPoint callPoint)
    {
        return new SceneServiceProxy(callPoint);
    }

    public static SceneServiceProxy Inst(string nodeIdId, string portId, object serviceId)
    {
        return new SceneServiceProxy(nodeIdId, portId, serviceId);
    }


    public CallPoint CallPoint { get; }

    public SceneServiceProxy(string nodeIdId, string portId, object serviceId)
    {
        CallPoint = new CallPoint(nodeIdId, portId, serviceId);
    }
    public SceneServiceProxy(CallPoint callPoint)
    {
        CallPoint = callPoint;
    }


    #region RPC

    public async Task<string> Test1(int i, string s)
    {
       Port port = Port.GetCurrent();
       Call call = port.MakeCall(CallPoint, 1, i, s);
       return await port.SendCall<string>(call);
    }

    public void Test3(int i, string s)
    {
       Port port = Port.GetCurrent();
       Call call = port.MakeCall(CallPoint, 2, i, s);
       port.SendCall(call);
    }

    public void t1(int i, string s)
    {
       Port port = Port.GetCurrent();
       Call call = port.MakeCall(CallPoint, 3, i, s);
       port.SendCall(call);
    }

    public async Task<string> t2(int i, string s)
    {
       Port port = Port.GetCurrent();
       Call call = port.MakeCall(CallPoint, 4, i, s);
       return await port.SendCall<string>(call);
    }

    public void t3(int i, string s)
    {
       Port port = Port.GetCurrent();
       Call call = port.MakeCall(CallPoint, 5, i, s);
       port.SendCall(call);
    }

    public async Task<string> t4(int i, string s)
    {
       Port port = Port.GetCurrent();
       Call call = port.MakeCall(CallPoint, 6, i, s);
       return await port.SendCall<string>(call);
    }

    #endregion
}
