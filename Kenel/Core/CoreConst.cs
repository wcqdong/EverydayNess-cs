namespace Core.Support;

public class CoreConst
{
    /// <summary>
    /// 心跳频率
    /// </summary>
    public static readonly int TickInterval = 2;

    /// <summary>
    /// 回调超时时间
    /// </summary>
    public static readonly int CallbackTimeout = 30000;

    /// <summary>
    /// 工程根目录
    /// </summary>
    public static readonly string BaseDir = $"{AppContext.BaseDirectory}../../../../";

}