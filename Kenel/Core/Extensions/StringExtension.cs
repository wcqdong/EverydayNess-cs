namespace Core.Extensions;

public static class StringExtension
{
    public static string ToUpperFirst(this string str)
    {
        if (String.IsNullOrEmpty(str))
        {
            return str;
        }

        return str[0].ToString().ToUpper() + str.Substring(1);
    }
}