using System.Diagnostics.CodeAnalysis;
using Core.Core;
using Google.Protobuf.WellKnownTypes;

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
        foreach (var node in Nodes)
        {
            if (node.Value == null)
            {
                continue;
            }
            string nodeId = node.Key;
            if (node.Value.Global != null)
            {
                foreach (var port in node.Value.Global)
                {
                    if (port.Value == null)
                    {
                        continue;
                    }
                    string portId = port.Key;
                    foreach (var serviceId in port.Value)
                    {
                        GlobalCallPoints.Add(serviceId, new CallPoint(nodeId, portId, serviceId));
                    }
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
    public string addr;
    public Dictionary<string, int> Normal { get; set; } = new();
    private Dictionary<string, List<string>> _global;
    public Dictionary<string, List<string>> Global
    {
        get => _global != null ? _global : new Dictionary<string, List<string>>();
        set => _global = value;
    }

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