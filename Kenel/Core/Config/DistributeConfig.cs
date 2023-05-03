using System.Diagnostics.CodeAnalysis;
using Core.Core;

namespace Core.Config;

public class DistributeConfig
{
    [NotNull]
    public static DistributeConfig Inst;

    public string Local { get; set; }
    public Dictionary<string, NodeConfig> Nodes { get; set; } = new ();

    public Dictionary<string, CallPoint> GlobalCallPoints { get; set; } = new();

    public void Init()
    {
        foreach (var item in Nodes)
        {
            if (item.Value == null || !item.Value.Global.Any())
            {
                continue;
            }
            string nodeId = item.Key;
            foreach (var item1 in item.Value.Global)
            {
                string portId = item.Key;
                foreach (var serviceId in item1.Value)
                {
                    GlobalCallPoints.Add(serviceId, new CallPoint(nodeId, portId, serviceId));
                }
            }
        }
    }

    public static NodeConfig GetLocalNode()
    {
        return Inst.Nodes[Inst.Local];
    }

}

public class NodeConfig
{
    public Dictionary<string, int> Normal { get; set; } = new();
    public Dictionary<string, List<string>> Global { get; set; } = new();
}

// public class StatelessConfig
// {
//     public string PortName { get; set; }
//     public int Num{ get; set; }
// }
// public class StatefulConfig
// {
//     public string PortName { get; set; }
//     public List<string> Services { get; set; } = new();
// }