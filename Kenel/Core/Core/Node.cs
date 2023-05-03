using System.Collections.Concurrent;
using Core.Support;
using Core.Utils;
using NetMQ;
using NetMQ.Sockets;

namespace Core.Core;

public class Node : IThread
{
    public string NodeId { get; }

    private ThreadHandler _thread;


    private readonly Dictionary<string, Port> _ports = new();

    private string _addr;
    private PullSocket _pullSocket;

    private long _now;

    protected readonly ConcurrentDictionary<string, RemoteNode> remoteNodes = new ();

    private ConcurrentQueue<RemoteCall> remoteCalls = new ();

    private readonly List<RemoteCall> _affirmRemoteCalls = new ();

    private byte[] _receiveBuffer = new byte[CoreConst.CALL_BUFFER_SIZE];

    public Node(string nodeId, string addr)
    {
        NodeId = nodeId;
        _addr = addr;

        _thread = new ThreadHandler(this);

        _pullSocket = new PullSocket();
        _pullSocket.Options.Linger = new TimeSpan(3000);

    }


    /// <summary>
    /// 这个函数多个线程都会调用，但是_ports不会有修改
    /// </summary>
    /// <param name="call"></param>
    public void Dispatch(string toNodeId, byte[] buffer, int offset)
    {
        // 本进程内
        if (toNodeId.Equals(NodeId))
        {
            InputStream input = new InputStream(buffer, 0, offset);
            DispatchLocal(input);
        }
        else
        {
            if(!remoteNodes.ContainsKey(toNodeId))
            {
                Console.WriteLine($"从{NodeId}节点没找到目标节点{toNodeId}");
                return;
            }

            remoteCalls.Enqueue(new RemoteCall(toNodeId, buffer, offset));
        }

    }

    public void DispatchLocal(InputStream input)
    {
        while (!input.IsAtEnd())
        {
            Call call = new Call();
            call.Read(input);

            if(!_ports.TryGetValue(call.To.PortId, out var toPort))
            {
                Console.WriteLine($"没找到目标线程 {call.To}");
                continue;
            }
            toPort.AddQueue(call);
        }
    }

    public void AddPort(Port port)
    {
        _ports.Add(port.PortId, port);
    }

    public void StartUp()
    {
        _thread.StartUp();

        _pullSocket.Bind(_addr);
    }

    public void OnStart()
    {

    }

    public void OnTick()
    {
        _now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        AffirmSendCall();

        TickSendCall();

        TickReceiveCall();
    }

    private void TickReceiveCall()
    {

        while(_pullSocket.TryReceiveFrameBytes(out var buffer))
        {
            if (buffer.Length <= 0)
            {
                break;
            }
            OnReceive(buffer);
        }
    }

    private void OnReceive(byte[] buffer)
    {
        CoreUtils.WriteLine($"{NodeId}节点收到消息");
        InputStream input = new InputStream(buffer, 0, buffer.Length);
        DispatchLocal(input);
    }

    private void AffirmSendCall()
    {
        while(remoteCalls.TryDequeue(out RemoteCall? call))
        {
           _affirmRemoteCalls.Add(call);
        }
    }

    private void TickSendCall()
    {
        foreach (var remoteCall in _affirmRemoteCalls)
        {
            try
            {
                SendRemoteCall(remoteCall);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        _affirmRemoteCalls.Clear();
    }

    private void SendRemoteCall(RemoteCall remoteCall)
    {
        if(!remoteNodes.TryGetValue(remoteCall.RemoteNodeId, out var remoteNode))
        {
            Console.WriteLine($"远程节点[{remoteNode}]不存在");
            return;
        }

        remoteNode.Send(remoteCall.Buffer);
    }

    public void OnStop()
    {
        throw new NotImplementedException();
    }

    public void AddRemoteNode(RemoteNode remoteNode)
    {
        if(!remoteNodes.TryAdd(remoteNode.RemoteNodeId, remoteNode))
        {
            Console.WriteLine($"{NodeId}节点重复添加了远程节点{remoteNode.RemoteNodeId}");
        }
    }
}