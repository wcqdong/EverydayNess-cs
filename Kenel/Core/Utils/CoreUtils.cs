using System.Reflection;
using Core.Extensions;

namespace Core.Utils;

public class CoreUtils
{
    public static void WriteLine(string message)
    {
        Console.WriteLine($"[ThreadId {Environment.CurrentManagedThreadId}] {message}");
    }

    public static string ServiceToFullName(string shortName)
    {
        return $"{shortName.ToUpperFirst()}Service";
    }

    public static Dictionary<string, Assembly> LoadServiceAssemblies(string path, string pattern = "")
    {
        Dictionary<string, Assembly> assemblies = new();

        DirectoryInfo dirInfo = new DirectoryInfo(path);
        DirectoryInfo[] dirs = dirInfo.GetDirectories(pattern);

        // 加载有状态服务的Assembly
        foreach (var serviceDirInfo in dirs)
        {
#if DEBUG
            assemblies.Add(serviceDirInfo.Name, Assembly.LoadFrom($"{serviceDirInfo}/bin/Debug/net8.0/{serviceDirInfo.Name}.dll"));
#else
            assemblies.Add(serviceDirInfo.Name, Assembly.LoadFrom($"{serviceDirInfo}/bin/Release/net8.0/{serviceDirInfo.Name}.dll"));
#endif
        }

        return assemblies;

    }
}