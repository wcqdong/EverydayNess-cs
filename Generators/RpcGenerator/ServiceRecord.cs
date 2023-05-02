using System.Reflection;
using System.Text;

namespace RpcGenerator;

public class ServiceRecord
{
    public string Name;
    public readonly HashSet<string> Namespaces = new ();
    public int ServiceType;
    public List<MethodRecord> Methods = new();

    public ServiceRecord(Type type, int serviceType)
    {
        Name = Transition.TransName(type);
        AddNamespace(type);
        ServiceType = serviceType;
    }

    public void AddNamespace(Type type)
    {
        string? v = Transition.TransNamespace(type);
        if (v != null)
        {
            Namespaces.Add(v);
        }
    }

    public void AddNamespace(string nameSpace)
    {
        Namespaces.Add(nameSpace);
    }
}


public class MethodRecord
{
    private ServiceRecord _serviceRecord;

    public string Name;
    public int Index;
    public string ReturnType;

    public List<ParamRecord> Parameters = new();

    public MethodRecord(ServiceRecord serviceRecord, string name, int index)
    {
        _serviceRecord = serviceRecord;
        Name = name;
        Index = index;
    }

    public void AddNamespace(Type type)
    {
        _serviceRecord.AddNamespace(type);
    }

    public void SetReturn(Type methodReturnType)
    {
        // 异步
        if (methodReturnType.FullName!.StartsWith("System.Threading.Tasks.Task"))
        {
            _serviceRecord.AddNamespace("System.Threading.Tasks.Task");
            if (methodReturnType.IsGenericType)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("async Task<");

                _serviceRecord.AddNamespace(methodReturnType.GenericTypeArguments[0]);
                sb.Append(Transition.TransName(methodReturnType.GenericTypeArguments[0]));

                sb.Append('>');
                ReturnType = sb.ToString();
            }
            else
            {
                _serviceRecord.AddNamespace(methodReturnType);
                ReturnType = $"async {Transition.TransName(methodReturnType)}";
            }

        }
        else
        {
            _serviceRecord.AddNamespace(methodReturnType);
            ReturnType = Transition.TransName(methodReturnType);
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