using Common.Gen.Proxy;
using Core.Attributes;
using Core.Core;
using Core.Utils;
using SceneService.Gen.RpcDispatcher;

namespace SceneService;

[Service(EServiceType.Normal)]
public class SceneService : Service
{

    private GameServiceProxy _gameServiceProxy;
    public SceneService(string serviceId) : base(serviceId)
    {
    }

    protected override void Init()
    {
        base.Init();
        // TODO 以后通过代码生成初始化 _rpcDispatcher
        ServiceRpcDispatcher = new SceneServiceRpcDispatcher();


        // TODO callpoint一定来源于一个地方，而不是随意手动创建的
        // TODO 比如逻辑服玩家一定持有场景服玩家的一个callpoint，操作场景玩家即对一个callpint进行rpc
        string toNodeId = Port.GetCurrent().Node.NodeId;
        string toPortId = "game0";
        string toServiceId = "game";
        CallPoint _sceneCallPoint = new CallPoint(toNodeId, toPortId, toServiceId);

        _gameServiceProxy = GameServiceProxy.Inst(_sceneCallPoint);
    }


    [Rpc]
    public async Task<string> Test1(int i, string s)
    {
        CoreUtils.WriteLine($"{ServiceId}::Test1 收到");

        CoreUtils.WriteLine($"{ServiceId}::Test1  ---调用-->  {_gameServiceProxy.CallPoint}::Test2");
        string result = await _gameServiceProxy.Test2(i, s);

        return result;
    }

    [Rpc]
    public void Test3(int i, string s)
    {
        CoreUtils.WriteLine($"{ServiceId}::Test3 收到");
    }

    [Rpc]
    public void t1(int i, string s)
    {

    }

    [Rpc]
    public string t2(int i, string s)
    {
        return "";
    }

    [Rpc]
    public async Task t3(int i, string s)
    {

    }

    [Rpc]
    public async Task<string> t4(int i, string s)
    {
        return "";
    }



}