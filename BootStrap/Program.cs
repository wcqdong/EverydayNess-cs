// See https://aka.ms/new-console-template for more information

using Core.Core;
using GameService;

class Program
{
    public static void Main()
    {
        // TODO 加载配置
        // TODO 加载程序集
        Node node = new Node("Node");
        StartUp.OnStartUp(node);
        SceneService.StartUp.OnStartUp(node);
        // TODO 反射程序集的StartUp

        // TODO 启动成功


        Console.Read();

    }
}