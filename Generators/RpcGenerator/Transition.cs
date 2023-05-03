namespace RpcGenerator;

public class Transition
{

    private static HashSet<string> ignoreNamspace = new()
    {
        "System",
        "System.Threading.Tasks"
    };

    private static readonly Dictionary<string, string> NameMapping = new()
    {
        {"Void", "void"},
        {"SByte", "sbyte"},
        {"Byte", "byte"},
        {"Int16", "short"},
        {"UInt16", "ushort"},
        {"Int32", "int"},
        {"UInt32", "uint"},
        {"Int64", "long"},
        {"UInt64", "ulong"},
        {"Char", "char"},
        {"Single", "float"},
        {"Double", "double"},
        {"Boolean", "bool"},
        {"Decimal", "decimal"},
        {"String", "string"},
        {"Object", "object"},
    };


    public static string? TransNamespace(Type type)
    {
        return ignoreNamspace.Contains(type.Namespace!) ? null : type.Namespace!;
    }


    public static string TransName(Type type)
    {
        if (NameMapping.TryGetValue(type.Name, out var name))
        {
            return name;
        }
        return type.Name;
    }
}