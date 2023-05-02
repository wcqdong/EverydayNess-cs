namespace Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute : Attribute
{
    public ServiceAttribute()
    {
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