using Core.Attributes;

namespace Common;

public class StartUp
{
    [Starter(0)]
    public static void OnStartUp()
    {
        // TODO 加载公共配置
    }
}