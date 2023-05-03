using NetMQ;
using NetMQ.Sockets;

namespace Core.Core;

public class RemoteNode
{
    // TODO 远程节点的状态管理  心跳检查


    private Node _localNode;
    public string RemoteNodeId { get; }
    private string _remoteAddr;

    private PushSocket _pushSocket;

    public RemoteNode(Node localNode, string remoteNodeId, string remoteAddr)
    {
        _localNode = localNode;
        RemoteNodeId = remoteNodeId;
        _remoteAddr = remoteAddr;

        _pushSocket = new PushSocket();
        _pushSocket.Options.Linger = new TimeSpan(3000);
        _pushSocket.Options.SendHighWatermark = 0;
        _pushSocket.Options.ReconnectInterval = new TimeSpan(2000);
        _pushSocket.Options.ReconnectIntervalMax = new TimeSpan(5000);
        _pushSocket.Connect(remoteAddr);
    }

    public void Send(byte[] remoteCallBuffer)
    {
        _pushSocket.SendFrame(remoteCallBuffer);
    }
}