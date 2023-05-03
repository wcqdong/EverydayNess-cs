using System.Reflection;
using Core.Attributes;
using Core.Core;
using Core.Utils;
using RpcGenerator.Extensions;
using RpcGenerator.Helpers;
using Scriban;

namespace RpcGenerator;

public class RpcGenerator
{
    /// <summary>
    /// 模板
    /// </summary>
    private Template _templateRpcDispatcher;

    private Template _templateServiceProxy;

    public RpcGenerator()
    {
        _templateRpcDispatcher = TemplateHelper.Create(GeneratorConst.TemplateRpcDispatcher);
        _templateServiceProxy = TemplateHelper.Create(GeneratorConst.TemplateServiceProxy);
    }

    public void Gen()
    {
        Dictionary<string, Assembly> assemblies = CoreUtils.LoadServiceAssemblies($"{GeneratorConst.BaseDir}Services", "*Service");

        List<ServiceRecord> serviceRecords = CollectRecord(assemblies);

        foreach (var record in serviceRecords)
        {
            string dispatcherName = $"{record.Name}RpcDispatcher.cs";
            Render(record, dispatcherName, $"{GeneratorConst.BaseDir}Services/{record.Name}/Gen/RpcDispatcher/", _templateRpcDispatcher).GetAwaiter().GetResult();
            string proxyName = $"{record.Name}Proxy.cs";
            Render(record, proxyName, $"{GeneratorConst.BaseDir}Common/Gen/Proxy/", _templateServiceProxy).GetAwaiter().GetResult();
        }
    }


    private static async Task Render(ServiceRecord record, string fileName, string filePath, Template template)
    {
        string output = $"{filePath}{fileName}";
        await template.WriteAsync(output, new
        {
            Record = record
        });

        Console.WriteLine($"ThreadId {Environment.CurrentManagedThreadId}【{fileName}.cs】生成成功!!");
    }

    private static List<ServiceRecord> CollectRecord(Dictionary<string, Assembly> assemblies)
    {
        List<ServiceRecord> serviceRecords = new();

        foreach (var item in assemblies)
        {
            ServiceRecord? serviceRecord = FindService(item.Value);
            if (serviceRecord == null)
            {
                continue;
            }
            serviceRecords.Add(serviceRecord);
        }

        return serviceRecords;
    }

    private static ServiceRecord? FindService(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            ServiceAttribute? attr = type.GetCustomAttribute<ServiceAttribute>();
            if (attr == null)
            {
                continue;
            }

            ServiceRecord serviceRecord = new ServiceRecord(type, (int)attr.Type);
            // 默认加一个ServiceRpcDispatcherBase的NameSpace
            serviceRecord.AddUsing(typeof(ServiceRpcDispatcherBase));
            FindRpcMethod(type, serviceRecord);

            return serviceRecord;
        }

        return null;
    }

    private static void FindRpcMethod(Type type, ServiceRecord serviceRecord)
    {
        int index = 1;
        foreach (var method in type.GetRuntimeMethods())
        {
            RpcAttribute? attr = method.GetCustomAttribute<RpcAttribute>();
            if (attr == null)
            {
                continue;
            }

            MethodRecord methodRecord = new MethodRecord(serviceRecord, method.Name, index++);
            methodRecord.SetReturn(method.ReturnType);

            foreach (var parameterInfo in method.GetParameters())
            {
                ParamRecord paramRecord = new ParamRecord(methodRecord, parameterInfo);
                methodRecord.Parameters.Add(paramRecord);
            }

            serviceRecord.Methods.Add(methodRecord);
        }

    }
}