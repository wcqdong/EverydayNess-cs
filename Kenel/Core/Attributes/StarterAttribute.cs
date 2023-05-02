namespace Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class StarterAttribute : Attribute
{
    public int Priority { get;}

    public StarterAttribute(int priority = 100)
    {
        Priority = priority;
    }
}
