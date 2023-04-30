using Common.Gen.Proxy;
using Core.Attributes;
using Core.Core;
using Core.Support;
using GameService.Gen.RcpDispatcher;

namespace GameService;

[Service]
public class GameService : Service
{

    private SceneServiceProxy _sceneServiceProxy;

    private readonly int SendMaxCount = 10000;
    private int _sendCount = 0;
    private int _loopCount = 1000;
    public GameService(string serviceId) : base(serviceId)
    {

    }

    protected override void Init()
    {
        base.Init();
        // TODO 以后通过代码生成初始化 _rpcDispatcher
        ProxyDispatcher = new GameServiceProxyDispatcher();


        // TODO callpoint一定来源于一个地方，而不是随意手动创建的
        // TODO 比如逻辑服玩家一定持有场景服玩家的一个callpoint，操作场景玩家即对一个callpint进行rpc
        string toNodeId = Port.GetCurrent().Node.NodeId;
        string toPortId = $"{toNodeId}.ScenePort0";
        string toServiceId = $"{toPortId}.SceneService0";
        CallPoint _sceneCallPoint = new CallPoint(toNodeId, toPortId, toServiceId);

        _sceneServiceProxy = new SceneServiceProxy(_sceneCallPoint);
    }

    public override void Tick(long now)
    {
        base.Tick(now);

        if (_sceneServiceProxy != null && _sendCount < SendMaxCount)
        {

            _sendCount++;
            for (int i = 0; i < _loopCount; i++)
            {
                // input
                CallSceneTest1();
                // CallSceneTest2();
            }

        }
    }

    private async void CallSceneTest1()
    {
        CoreUtils.WriteLine($"{ServiceId}::CallSceneTest1  ---调用-->  {_sceneServiceProxy.CallPoint}::Test1");

        string result = await _sceneServiceProxy.Test1(1, "2");
        CoreUtils.WriteLine($"{ServiceId}::CallSceneTest1 收到返回值={result}");
        CoreUtils.WriteLine($"{ServiceId}::CallSceneTest1 执行回调");

        CoreUtils.WriteLine($"{ServiceId}::CallSceneTest1  ---调用-->  {_sceneServiceProxy.CallPoint}::Test3");
        _sceneServiceProxy.Test3(1, "2");
    }

    // private async void CallSceneTest2()
    // {
    //     CoreUtils.WriteLine($"{ServiceId}::CallSceneTest2 调用");
    //
    //     string result = await _sceneServiceProxy.Test2(1, "2");
    //
    //     CoreUtils.WriteLine($"{ServiceId}::CallSceneTest2 回调 返回值={result}");
    // }



    [Rpc]
    public void Test1()
    {
    }

    [Rpc]
    public string Test2(int a, string b)
    {
        string result = "success";
        CoreUtils.WriteLine($"{ServiceId}::Test2 收到");
        CoreUtils.WriteLine($"{ServiceId}::Test2 返回给调用者，返回值的={result}");
        return result;
    }

    [Rpc]
    public string Test3(int a, string b)
    {
        return "";
    }

    [Rpc]
    public async Task<string> Test4(int a, string b)
    {
        string s = await AwaitGameTest4();
        return s;
    }

    public Task<string> AwaitGameTest4()
    {
        return Task.FromResult("");
    }

}