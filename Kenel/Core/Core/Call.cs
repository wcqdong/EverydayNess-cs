namespace Core.Core;

public class Call
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
    public uint CallBackId;

    public object result;


    public override string ToString()
    {
        return $"{FromNode}.{FromPort} --> {To} :: {MethodKey}";
    }
}