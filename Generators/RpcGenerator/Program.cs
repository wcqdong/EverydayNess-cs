// See https://aka.ms/new-console-template for more information

internal class Program
{

    public static void Main()
    {

        global::RpcGenerator.RpcGenerator generator = new global::RpcGenerator.RpcGenerator();
        generator.Gen();

    }
}