namespace RpcGenerator;

public class Transition
{
    public static string? TransNamespace(Type type)
    {
        return type.Namespace!;
    }


    public static string TransName(Type type)
    {
        return type.Name;
    }
}