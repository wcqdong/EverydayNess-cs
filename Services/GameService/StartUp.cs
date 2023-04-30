using Core.Core;

namespace GameService;

public class StartUp
{
    public static void OnStartUp(Node node)
    {
        // 启动10个场景线程，没个场景线程10个Service
        for (int i = 0; i < 1; i++)
        {
            Port port = new Port($"{node.NodeId}.GamePort{i}");
            for (int j = 0; j < 1; j++)
            {
                // TODO serviceId应该由分布式ID生成
                GameService service = new GameService($"{port.PortId}.GameService{j}");
                port.AddService(service);
            }

            port.StartUp(node);
        }
    }
}