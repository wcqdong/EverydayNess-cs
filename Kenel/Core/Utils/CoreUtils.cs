using Core.Extensions;

namespace Core.Utils;

public class CoreUtils
{
    public static void WriteLine(string message)
    {
        // Console.WriteLine($"[ThreadId {Environment.CurrentManagedThreadId}] : {message}");
    }

    public static string ServiceToFullName(string shortName)
    {
        return $"{shortName.ToUpperFirst()}Service";
    }
}