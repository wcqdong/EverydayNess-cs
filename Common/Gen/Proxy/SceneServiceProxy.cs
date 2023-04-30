using Core.Core;

namespace Common.Gen.Proxy;

public class SceneServiceProxy
{
    // private static SceneServiceProxy _proxy;

    public CallPoint CallPoint { get; }

    // public static SceneServiceProxy Inst()
    // {
    //     if (_proxy == null)
    //     {
    //         _proxy = new SceneServiceProxy("SceneNode", "port1", "scene");
    //     }
    //
    //     return _proxy;
    // }

    public static SceneServiceProxy Inst(CallPoint callPoint)
    {
        return new SceneServiceProxy(callPoint);
    }

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
        int methodId = 1;

        Port port = Port.GetCurrent();

        Call call = new Call
        {
            FromNode = port.Node.NodeId,
            FromPort = port.PortId,
            To = CallPoint,
            MethodKey = methodId,
            MethodParams = new object[]{ i, s}
        };

        Task<string> task = port.AddCall<string>(call);

        return await task;
    }

    public void Test3(int i, string s)
    {
        int methodId = 3;

        Port port = Port.GetCurrent();

        Call call = new Call
        {
            FromNode = port.Node.NodeId,
            FromPort = port.PortId,
            To = CallPoint,
            MethodKey = methodId,
            MethodParams = new object[]{ i, s}
        };

        port.AddCall(call);
    }

    #endregion



}