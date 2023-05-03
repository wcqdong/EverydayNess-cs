
using EnumsNET;

namespace RpcGenerator.Extensions;

public static class DirectoryExtension
{
    /// <summary>
    /// 如果目录为只读则设置为非只读
    /// </summary>
    /// <param name="dirInfo">目录信息</param>
    /// <param name="recursive">是否遍历设置子目录，默认false</param>
    public static void ToNotReadOnly(this DirectoryInfo dirInfo, bool recursive = false)
    {
        if (!dirInfo.Exists || !dirInfo.Attributes.HasAllFlags(FileAttributes.ReadOnly))
        {
            return;
        }

        dirInfo.Attributes &= ~FileAttributes.ReadOnly;
        if (!recursive) return;

        var dirInfos = dirInfo.GetDirectories();
        foreach (var itemInfo in dirInfos)
        {
            itemInfo.ToNotReadOnly(recursive);
        }
    }
}