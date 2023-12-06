using Core.Core.Serialize;

namespace Core.Core;

public interface ISerialize
{
    public void Write(OutputStream stream);
    public void Read(InputStream stream);
}