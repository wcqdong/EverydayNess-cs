using Google.Protobuf;

namespace Core.Core.Serialize;

public class OutputStream
{
    public CodedOutputStream Stream { get; set; }

    public byte[] Buffer { get; }
    public int MaxLength { get; }

    public OutputStream(byte[] bytes)
    {
        Buffer = bytes;
        MaxLength = bytes.Length;
        Stream = new CodedOutputStream(Buffer);
    }

    public void WriteObject(object? v)
    {
        if (v == null)
        {
            WriteNull();
        }
        else if (v is int i)
        {
            WriteInt(i);
        }else if (v is long l)
        {
            WriteLong(l);
        }else if (v is string s)
        {
            WriteString(s);
        }
        else
        {
            throw new Exception("序列化时暂时不支持的类型");
        }
    }

    private void WriteNull()
    {
        Stream.WriteInt32(SerializeType.NULL);
    }

    public void WriteLong(long v)
    {
        Stream.WriteInt32(SerializeType.LONG);
        Stream.WriteSInt64(v);
    }

    public void WriteInt(int v)
    {
        Stream.WriteInt32(SerializeType.INT);
        Stream.WriteSInt32(v);
    }

    public void WriteString(string v)
    {
        Stream.WriteInt32(SerializeType.STRING);
        Stream.WriteString(v);
    }

    public int GetLength() {
        return MaxLength - Stream.SpaceLeft;
    }

    public void Reset()
    {
        Stream = new CodedOutputStream(Buffer);
    }
}