namespace Core;

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
    /// 发送call缓冲区大小
    /// </summary>
    public const int CALL_BUFFER_SIZE = 2 * 1024 * 1024;


    /// <summary>
    /// 工程根目录
    /// </summary>
    public static readonly string BaseDir = $"{AppContext.BaseDirectory}../../";

}