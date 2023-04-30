namespace Core.Core;

public class Call
{
    // TODO 从分布式ID获取，暂时不用
    public long Id;

    // 0=发送  1=返回
    public int Type;

    //
    public string FromNode;
    public string FromPort;
    // public object FromService;

    public CallPoint To;


    // 调用的rpc的函数
    public int MethodKey;
    // rpc所需的参数
    public object[] MethodParams;

    // 回调
    public uint CallBackId;

    // public bool NeedResult;

    public object result;
}