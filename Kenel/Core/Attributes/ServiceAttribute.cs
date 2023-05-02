namespace Core.Attributes;


public enum EServiceType
{
    Normal = 1,
    Global = 2,

}
[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute : Attribute
{
    public EServiceType Type { get; }

    public ServiceAttribute(EServiceType type)
    {
        Type = type;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class SingleServiceAttribute : Attribute
{
    public string ServiceId { get; }

    public SingleServiceAttribute(string serverId)
    {
        ServiceId = serverId;
    }
}