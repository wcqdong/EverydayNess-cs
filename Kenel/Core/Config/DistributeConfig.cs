using System.Diagnostics.CodeAnalysis;

namespace Core.Config;

public class DistributeConfig
{
    [NotNull]
    public static DistributeConfig Inst;

    public string Local { get; set; }
    public Dictionary<string, NodeConfig> Nodes { get; set; }= new ();


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