using System.Text;

namespace Gen;

public static class FileHelper
{

    /// <summary>
    /// 如果文件为只读则设置为非只读
    /// </summary>
    /// <param name="filepath">文件路径</param>
    private static void ToNotReadOnly(string filepath) =>
        new FileInfo(filepath).ToNotReadOnly();

    /// <summary>
    /// 写入所有文本行内容，异步方法
    /// </summary>
    /// <param name="filepath">文件路径</param>
    /// <param name="contents">文本内容</param>
    /// <param name="encoding">编码</param>
    public static Task WriteAllLinesAsync(string filepath, IEnumerable<string> contents,
        Encoding? encoding = null)
    {
        ToNotReadOnly(filepath);
        return File.WriteAllLinesAsync(filepath, contents, encoding ?? EncodingConst.Utf8WithoutBom);
    }

    /// <summary>
    /// 写入所有文本内容，异步方法
    /// </summary>
    /// <param name="filepath">文件路径</param>
    /// <param name="contents">文本内容</param>
    /// <param name="encoding">编码，默认UTF8</param>
    public static Task WriteAllTextAsync(string filepath, string contents, Encoding? encoding = null)
    {
        ToNotReadOnly(filepath);
        return File.WriteAllTextAsync(filepath, contents, encoding ?? EncodingConst.Utf8WithoutBom);
    }
}