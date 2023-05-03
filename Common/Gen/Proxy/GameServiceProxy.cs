using Core.Config;
using Core.Core;

namespace Common.Gen.Proxy;

public class GameServiceProxy
{
    public static GameServiceProxy Inst(CallPoint callPoint)
    {
        return new GameServiceProxy(callPoint);
    }

    public static GameServiceProxy Inst(string nodeIdId, string portId, object serviceId)
    {
        return new GameServiceProxy(nodeIdId, portId, serviceId);
    }


    public CallPoint CallPoint { get; }

    private GameServiceProxy(string nodeIdId, string portId, object serviceId)
    {
        CallPoint = new CallPoint(nodeIdId, portId, serviceId);
    }
    private GameServiceProxy(CallPoint callPoint)
    {
        CallPoint = callPoint;
    }


    #region RPC

    public void Test1()
    {
       Port port = Port.GetCurrent();
       Call call = port.MakeCall(CallPoint, 1);
       port.SendCall(call);
    }

    public async Task<string> Test2(int a, string b)
    {
       Port port = Port.GetCurrent();
       Call call = port.MakeCall(CallPoint, 2, a, b);
       return await port.SendCall<string>(call);
    }

    public async Task<string> Test3(int a, string b)
    {
       Port port = Port.GetCurrent();
       Call call = port.MakeCall(CallPoint, 3, a, b);
       return await port.SendCall<string>(call);
    }

    public async Task<string> Test4(int a, string b)
    {
       Port port = Port.GetCurrent();
       Call call = port.MakeCall(CallPoint, 4, a, b);
       return await port.SendCall<string>(call);
    }

    #endregion
}
