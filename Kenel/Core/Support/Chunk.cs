using Core.Core;

namespace Core.Support;

public class Chunk
{
    public byte[] Buffer { get; }
    public int Offset { get; }
    public int Length { get; }

    public Chunk()
    {
    }
}