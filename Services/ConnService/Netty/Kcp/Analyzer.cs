namespace ConnService.Netty.Kcp;

public class Analyzer
{
    public static ulong ReceiveCount = 0;

    public static ulong ReceiveLen = 0;

    public static void Receive()
    {
        Interlocked.Increment(ref ReceiveCount);
    }

    public static void ReceiveLength(ulong msgReadableBytes)
    {
        Interlocked.Add(ref ReceiveLen, msgReadableBytes);
    }
}