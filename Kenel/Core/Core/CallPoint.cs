namespace Core.Core;

public class CallPoint
{
    public string NodeId { get; set; }
    public string PortId{ get; set; }
    public object ServiceId{ get; set; }

    public CallPoint()
    {
    }

    public CallPoint(string nodeId, string portId, object serviceId)
    {
        NodeId = nodeId;
        PortId = portId;
        ServiceId = serviceId;
    }

    public override string ToString()
    {
        return $"{NodeId}.{PortId}.{ServiceId}";
    }
}