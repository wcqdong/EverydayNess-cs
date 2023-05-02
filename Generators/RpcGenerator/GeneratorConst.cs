namespace RpcGenerator;

public class GeneratorConst
{
    private static readonly string AppDir = AppContext.BaseDirectory.TrimEnd('/', '\\');

    public static readonly string BaseDir = $"{AppDir}/../../../../../";


    /// <summary>
    /// 模板目录
    /// </summary>
    internal static readonly string TemplateDir = Path.Combine(AppDir, "Templates");

    /// <summary>
    /// 模板文件
    /// </summary>
    internal static readonly string TemplateRpcDispatcher = $"{TemplateDir}/RpcDispatcher.sbn";

    /// <summary>
    /// 模板文件
    /// </summary>
    internal static readonly string TemplateServiceProxy = $"{TemplateDir}/ServiceProxy.sbn";
}