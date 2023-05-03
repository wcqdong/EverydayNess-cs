namespace Core.Core;

public class RemoteCall
{
    public string RemoteNodeId { get; }

    public byte[] Buffer { get; }

    public RemoteCall(string remoteNodeId, byte[] buffer, int offset)
    {
        RemoteNodeId = remoteNodeId;
        // 拷贝
        Buffer = new byte[offset];
        Array.Copy(buffer, Buffer, offset);
    }
}