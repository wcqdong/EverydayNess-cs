// See https://aka.ms/new-console-template for more information

using System.Reflection;
using BootStrap;
using Core.Attributes;
using Core.Config;
using Core.Core;
using Core.Support;
using Core.Utils;

class Program
{
    public static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            throw new Exception("启动参数[0]必须指定分布式配置");
        }

        // step1. 分布式配置
        // --------------------------
        LoadDistributeConfig($"{CoreConst.BaseDir}../Config/DistributeConfig.yml");
        DistributeConfig.Inst.Local = args[0];

        // step2. 加载Service程序集
        // --------------------------
        Dictionary<string, Assembly> assemblies = LoadServiceAssembly();

        // step3. 初始化Service程序集（并非初始化了Service，Service在BuildDistributeLocal中启动）
        // --------------------------
        InitAssembly();

        // step4. 构建分布式
        // --------------------------
        // step4.1 构建本地Node
        // 创建一个进程节点
        NodeConfig nodeConfig = DistributeConfig.GetLocalNode();
        Node node = new Node(DistributeConfig.Inst.Local, nodeConfig.addr);
        BuildDistributeLocal(node, assemblies);
        // step4.2 构建分布式Node
        BuildDistributeGlobal(node);

        // TODO step5. 监听关闭
        // --------------------------
        Console.Read();

    }

    public static void LoadDistributeConfig(string filePath)
    {
        DistributeConfig.Inst = YamlUtils.Load<DistributeConfig>(filePath);
        DistributeConfig.Inst.Init();
    }

    private static void InitAssembly()
    {
        // 收集程序的StartUp启动函数
        List<MethodInfo> startUpMethods = CollectStartup();

        foreach (var method in startUpMethods)
        {
            object obj = Activator.CreateInstance(method.DeclaringType!)!;
            method.Invoke(obj, new object?[]{});
        }
    }

    private static Dictionary<string, Assembly> LoadServiceAssembly()
    {
        Dictionary<string, Assembly> assemblies = new();

        DirectoryInfo dir = new DirectoryInfo($"{CoreConst.BaseDir}Services");
        DirectoryInfo[] dirs = dir.GetDirectories("*Service");

        NodeConfig localNode = DistributeConfig.GetLocalNode();

        if (localNode.Global != null)
        {
            // 加载有状态服务的Assembly
            foreach (var shortNames in localNode.Global.Values)
            {
                if (shortNames == null)
                {
                    continue;
                }
                LoadServiceAssembly(shortNames, assemblies, dirs);
            }
        }


        // 加载无状态服务的Assembly
        LoadServiceAssembly(localNode.Normal.Keys.ToList(), assemblies, dirs);

        return assemblies;

    }

    private static void LoadServiceAssembly(List<string> shortNames, Dictionary<string, Assembly> assemblies, DirectoryInfo[] dirs)
    {
        foreach (var shortName in shortNames)
        {
            string fullName = CoreUtils.ServiceToFullName(shortName);
            foreach (var serviceDirInfo in dirs)
            {
                if (serviceDirInfo.Name.Equals(fullName))
                {
#if DEBUG
                    PluginAssemblyLoadContext loadContext = PluginAssemblyLoadContext.LoadPlugin($"{serviceDirInfo}/{serviceDirInfo.Name}.dll",
                        $"{serviceDirInfo}/{serviceDirInfo.Name}.pdb");
                    foreach (var assembly in loadContext.Assemblies)
                    {
                        assemblies.Add(fullName, assembly);
                    }
                    // assemblies.Add(fullName, Assembly.LoadFrom($"{serviceDirInfo}/bin/Debug/net8.0/{serviceDirInfo.Name}.dll"));
#else
                    assemblies.Add(fullName, Assembly.LoadFrom($"{serviceDirInfo}/bin/Release/net8.0/{serviceDirInfo.Name}.dll"));
#endif
                }
            }

        }
    }

    private static void BuildDistributeGlobal(Node node)
    {
        // TODO 构建分布式关系，现在通过本地配置构建分布式，以后通过服务发现实现
        foreach (var item in DistributeConfig.Inst.Nodes)
        {
            if (item.Key.Equals(DistributeConfig.Inst.Local))
            {
                continue;
            }
            // TODO 构建分布式
            string remoteNodeId = item.Key;
            NodeConfig remoteNodeConfig = item.Value;

            RemoteNode remoteNode = new RemoteNode(node, remoteNodeId, remoteNodeConfig.addr);
            node.AddRemoteNode(remoteNode);
        }
    }

    private static void BuildDistributeLocal(Node node, Dictionary<string, Assembly> assemblies)
    {
        // 获得所有Service工程
        DirectoryInfo dir = new DirectoryInfo($"{CoreConst.BaseDir}Services");
        DirectoryInfo[] dirs = dir.GetDirectories("*Service");

        // 根据本地分布式配置启动线程和服务
        NodeConfig localNode = DistributeConfig.GetLocalNode();

        // 启动有状态服务
        foreach (var item in localNode.Global)
        {
            if (item.Value == null)
            {
                continue;
            }
            Port port = new Port(item.Key);
            foreach (string serviceName in item.Value)
            {
                AddService(port, serviceName, dirs, assemblies);
            }

            // 线程启动
            port.StartUp(node);
        }


        foreach (var item in localNode.Normal)
        {
            for (int i = 0; i < item.Value; i++)
            {
                Port port = new Port($"{item.Key}{i}");

                AddService(port, item.Key, dirs, assemblies);

                port.StartUp(node);
            }
        }

        node.StartUp();
    }

    private static bool AddService(Port port, string shortName, DirectoryInfo[] dirs, Dictionary<string, Assembly> assemblies)
    {
        string fullName = CoreUtils.ServiceToFullName(shortName);
        bool funded = false;
        foreach (var serviceDirInfo in dirs)
        {
            if (!serviceDirInfo.Name.Equals(fullName))
            {
                continue;
            }

            Assembly assembly = assemblies[fullName];
            Type? serviceType = assembly.GetType($"{fullName}.{fullName}");
            if (serviceType == null)
            {
                Console.WriteLine($"配置了{fullName}服务，但并没找到{fullName}.{fullName}服务类");
                return false;
            }

            // new Service
            Service service = (Service)Activator.CreateInstance(serviceType, shortName)!;
            port.AddService(service);

            funded = true;
        }

        if (!funded)
        {
            throw new Exception($"配置了{shortName}服务，但并没找到{fullName}工程");
        }


        return true;
    }

    /// <summary>
    /// 收集Startup函数
    /// </summary>
    /// <returns></returns>
    private static List<MethodInfo> CollectStartup()
    {
        List<MethodInfo> startUpMethods = new();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            string name = assembly.GetName().Name!;
            Type? startUp = assembly.GetType(name + ".StartUp");
            if (startUp == null)
            {
                continue;
            }

            var methods = startUp.GetMethods().Where(method => method.GetCustomAttribute<StarterAttribute>() != null);
            if (!methods.Any())
            {
                continue;
            }

            MethodInfo method = methods.First();
            startUpMethods.Add(method);
        }

        // 按优先级排序
        startUpMethods.Sort((m1, m2) =>
        {
            StarterAttribute attr1 = m1.GetCustomAttribute<StarterAttribute>()!;
            StarterAttribute attr2 = m2.GetCustomAttribute<StarterAttribute>()!;
            return attr1.Priority - attr2.Priority;
        });
        return startUpMethods;
    }
}