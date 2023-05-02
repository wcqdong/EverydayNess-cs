
namespace Gen;

public static class FileExtension
{
    public static void ToNotReadOnly(this FileInfo fileInfo)
    {
        if (!fileInfo.Exists || !fileInfo.IsReadOnly) return;

        fileInfo.Directory?.ToNotReadOnly();
        fileInfo.IsReadOnly = false;
    }

    public static void DeleteEvenReadOnly(this FileInfo fileInfo)
    {
        fileInfo.ToNotReadOnly();
        fileInfo.Delete();
    }
}