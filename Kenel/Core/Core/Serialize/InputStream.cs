using Core.Support;
using Google.Protobuf;

namespace Core.Core;

public class InputStream
{
    private CodedInputStream _stream;

    public InputStream(Chunk chunk) : this(chunk.Buffer, chunk.Offset, chunk.Length){

    }

    public T ReadObject<T>()
    {
        object result;
        int type = _stream.ReadInt32();
        if (type == SerializeType.NULL)
        {
            result = null;
        }
        else if (type == SerializeType.INT)
        {
            result = DoReadInt();
        }else if (type == SerializeType.LONG)
        {
            result = DoReadLong();
        }else if (type == SerializeType.STRING)
        {
            result = DoReadString();
        }
        else
        {
            throw new Exception("反序列化时暂时不支持的类型");
        }

        return (T)result;
    }

    public InputStream(byte[] buffer, int offset, int length) {
        _stream = new CodedInputStream(buffer, offset, length);
    }

    public long ReadLong()
    {
        int type = _stream.ReadInt32();
        return DoReadLong();
    }

    public int ReadInt()
    {
        int type = _stream.ReadInt32();
        return DoReadInt();
    }

    public string ReadString()
    {
        int type = _stream.ReadInt32();
        return DoReadString();
    }


    private int DoReadInt()
    {
        return _stream.ReadSInt32();
    }

    public string DoReadString()
    {
        return _stream.ReadString();
    }

    private long DoReadLong()
    {
        return _stream.ReadSInt64();
    }

    public bool IsAtEnd()
    {
        return _stream.IsAtEnd;
    }
}