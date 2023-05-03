namespace Core.Core;

public class CallPoint : ISerialize
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

    public void Write(OutputStream stream)
    {
        stream.WriteString(NodeId);
        stream.WriteString(PortId);
        stream.WriteObject(ServiceId);
    }

    public void Read(InputStream stream)
    {
        NodeId = stream.ReadString();
        PortId = stream.ReadString();
        ServiceId = stream.ReadObject<string>();
    }
}