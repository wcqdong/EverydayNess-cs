using Core.Core.Serialize;

namespace Core.Core;

public class Call : ISerialize
{
    // TODO 从分布式ID获取，暂时不用
    public long Id;

    // 0=发送  1=返回
    public int Type;

    //  FROM
    public string FromNode;
    public string FromPort;

    // TO
    public CallPoint To;

    // 调用的rpc的函数
    public int MethodKey;
    // rpc所需的参数
    public object[] MethodParams;

    // 回调
    public int CallBackId;
    // rpc返回的信息
    public object result;


    public override string ToString()
    {
        return $"{FromNode}.{FromPort} --> {To} :: {MethodKey}";
    }

    public void Write(OutputStream stream)
    {
        stream.WriteLong(Id);
        stream.WriteInt(Type);
        stream.WriteString(FromNode);
        stream.WriteString(FromPort);
        To.Write(stream);
        stream.WriteInt(MethodKey);

        if (MethodParams == null)
        {
            stream.WriteInt(0);
        }
        else
        {
            stream.WriteInt(MethodParams.Length);
            foreach (var mp in MethodParams)
            {
                stream.WriteObject(mp);
            }
        }

        stream.WriteInt(CallBackId);

        stream.WriteObject(result);
    }

    public void Read(InputStream stream)
    {

        Id = stream.ReadLong();
        Type = stream.ReadInt();
        FromNode = stream.ReadString();
        FromPort = stream.ReadString();
        To = new CallPoint();
        To.Read(stream);
        MethodKey = stream.ReadInt();

        int length = stream.ReadInt();
        MethodParams = new object[length];
        for (int i=0; i<length; ++i)
        {
            MethodParams[i] = stream.ReadObject<object>();
        }

        CallBackId = stream.ReadInt();

        result = stream.ReadObject<object>();
    }
}