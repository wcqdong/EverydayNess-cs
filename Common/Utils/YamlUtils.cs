using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Core.Utils;

public class YamlUtils
{
    public static T Load<T>(string filePath)
    {
        StreamReader? input = null;
        try
        {
            input = File.OpenText(filePath);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            T result = deserializer.Deserialize<T>(input);
            return result;

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            input?.Close();
        }
    }
}