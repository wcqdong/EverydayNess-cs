
using System.Text;

namespace Gen;

/// <summary>
/// 编码常量类
/// </summary>
public static class EncodingConst
{
    /// <summary>
    /// Utf8，无bom
    /// </summary>
    public static readonly Encoding Utf8WithoutBom = new UTF8Encoding(false, true);

    /// <summary>
    /// Utf8，有bom
    /// </summary>
    public static readonly Encoding Utf8WithBom = Encoding.UTF8;
}