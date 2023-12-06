// See https://aka.ms/new-console-template for more information

using NettyClient;

public class Program
{
    public static void Main()
    {
        KcpClient.Start();
        // TcpClient.Start();
    }
}