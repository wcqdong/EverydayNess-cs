namespace ConnService.Netty.Kcp.KcpCore;

public static class KcpUtils
{

    public static long Now()
    {
        return DateTime.UtcNow.Millisecond;
    }

    public static uint GetTick() => unchecked((uint)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

}