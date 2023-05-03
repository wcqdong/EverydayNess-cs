using System.Reflection;
using System.Text;
using Core.Config;
using Core.Core;
using Core.Extensions;

namespace RpcGenerator;

public class ServiceRecord
{
    public string Name;
    public string ServiceId;
    public readonly HashSet<string> DispatcherUsings = new ();
    public readonly HashSet<string> ProxyUsings = new ();
    public int ServiceType;
    public List<MethodRecord> Methods = new();

    public ServiceRecord(Type type, int serviceType)
    {
        Name = Transition.TransName(type);
        ServiceId = Name.Replace("Service", "").ToLowerFirst();
        // proxy需要的类
        ProxyUsings.Add(Transition.TransNamespace(typeof(DistributeConfig))!);

        ServiceType = serviceType;
    }

    public void AddUsing(Type type)
    {
        string? v = Transition.TransNamespace(type);
        if (v == null) return;
        DispatcherUsings.Add(v);
        ProxyUsings.Add(v);
    }

    public void AddUsing(string nameSpace)
    {
        ProxyUsings.Add(nameSpace);
    }
}


public class MethodRecord
{
    private ServiceRecord _serviceRecord;

    public string Name;
    public int Index;
    public string ReturnType;
    public string AsyncReturnType;
    public int CallType;

    public List<ParamRecord> Parameters = new();

    public MethodRecord(ServiceRecord serviceRecord, string name, int index)
    {
        _serviceRecord = serviceRecord;
        Name = name;
        Index = index;
    }

    public void AddNamespace(Type type)
    {
        _serviceRecord.AddUsing(type);
    }

    public void SetReturn(Type methodReturnType)
    {
        // 异步
        if (methodReturnType.FullName!.StartsWith("System.Threading.Tasks.Task"))
        {
            if (methodReturnType.IsGenericType)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("async Task<");

                _serviceRecord.AddUsing(methodReturnType.GenericTypeArguments[0]);
                sb.Append(Transition.TransName(methodReturnType.GenericTypeArguments[0]));

                sb.Append('>');
                ReturnType = sb.ToString();
                CallType = (int)EReturnType.ASYNC_OBJECT;
                AsyncReturnType = Transition.TransName(methodReturnType.GenericTypeArguments[0]);
            }
            else
            {
                _serviceRecord.AddUsing(methodReturnType);
                ReturnType = "void";
                CallType = (int)EReturnType.ASYNC_VOID;
            }
        }
        else
        {
            _serviceRecord.AddUsing(methodReturnType);
            if (methodReturnType.FullName.Equals("System.Void"))
            {
                ReturnType = "void";
                CallType = (int)EReturnType.VOID;
            }
            else
            {

                ReturnType = $"async Task<{Transition.TransName(methodReturnType)}>";
                AsyncReturnType = Transition.TransName(methodReturnType);
                CallType = (int)EReturnType.OBJECT;
            }
        }
    }
}

public class ParamRecord
{
    private MethodRecord _methodRecord;

    public string type;
    public string name;

    public ParamRecord(MethodRecord methodRecord, ParameterInfo parameterInfo)
    {
        _methodRecord = methodRecord;
        _methodRecord.AddNamespace(parameterInfo.ParameterType);
        type = Transition.TransName(parameterInfo.ParameterType);
        name = parameterInfo.Name!;
    }
}