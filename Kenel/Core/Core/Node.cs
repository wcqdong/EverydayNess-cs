namespace Core.Core;

public class Node
{
    public string NodeId { get; }

    private readonly Dictionary<string, Port> _ports = new();

    public Node(string nodeId)
    {
        NodeId = nodeId;
    }


    /// <summary>
    /// 这个函数多个线程都会调用，但是_ports不会有修改
    /// </summary>
    /// <param name="call"></param>
    public void Dispatch(Call call)
    {
        if(!_ports.TryGetValue(call.To.PortId, out var toPort))
        {
            Console.WriteLine("目标线程不存在");
            return;
        }

        toPort.AddQueue(call);
    }

    public void AddPort(Port port)
    {
        _ports.Add(port.PortId, port);
    }
}