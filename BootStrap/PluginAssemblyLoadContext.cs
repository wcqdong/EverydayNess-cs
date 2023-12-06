using System.Reflection;
using System.Runtime.Loader;

namespace BootStrap;

public class PluginAssemblyLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;

    private readonly AssemblyLoadContext _defaultLoadContext = GetLoadContext(Assembly.GetExecutingAssembly()) ?? Default;

    public PluginAssemblyLoadContext(string pluginPath) : base(isCollectible: true)
    {
        _resolver = new AssemblyDependencyResolver(pluginPath);
    }

    protected override Assembly? Load(AssemblyName name)
    {
        var assembly = LoadFromDefaultContext(name);
        if (assembly != null)
        {
            return assembly;
        }

        var assemblyPath = _resolver.ResolveAssemblyToPath(name);
        if (assemblyPath == null)
            return null;

        assembly = LoadFromAssemblyPath(assemblyPath);

        return assembly;
    }

    private Assembly? LoadFromDefaultContext(AssemblyName name)
    {
        try
        {
            return _defaultLoadContext.LoadFromAssemblyName(name);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static PluginAssemblyLoadContext LoadPlugin(string assemblyPath, string pdbPath)
    {
        var ctx = new PluginAssemblyLoadContext(assemblyPath);

        using var pdb = new FileStream(pdbPath, FileMode.Open, FileAccess.Read);
        using var file = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read);

        var assembly = ctx.LoadFromStream(file, pdb);

        // var pluginType = assembly.GetType(pluginTypeName);
        // if (pluginType is null)
        // {
        //     ctx.Unload();
        //
        //     return null;
        // }
        //
        // if (!typeof(IPlugin).IsAssignableFrom(pluginType))
        // {
        //     return null;
        // }
        //
        // var instance = Activator.CreateInstance(pluginType);
        // if (instance is not IPlugin plugin)
        // {
        //     ctx.Unload();
        //
        //     return null;
        // }

        return ctx;
    }
}