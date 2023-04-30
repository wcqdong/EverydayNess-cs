namespace Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class RpcAttribute : Attribute
{
    public RpcAttribute()
    {
    }
}
