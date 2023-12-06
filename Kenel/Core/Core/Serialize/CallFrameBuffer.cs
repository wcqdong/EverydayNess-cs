using Core.Core.Serialize;
using Core.Support;
using Google.Protobuf;

namespace Core.Core;

public class CallFrameBuffer
{

    private OutputStream _stream = new (new byte[CoreConst.CALL_BUFFER_SIZE]);

    public CallFrameBuffer()
    {

    }

    public bool WriteCall(Call call)
    {
        int offset = _stream.GetLength();
        int length = _stream.MaxLength;
        try
        {
            call.Write(_stream);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // TODO 恢复
            // _stream.Stream = new CodedOutputStream(_stream.Buffer, offset, length - offset);
            return false;
        }

    }

    public void Flush(string nodeId, Node node)
    {
        try
        {
            if (_stream.GetLength() == 0)
            {
                return;
            }
            node.Dispatch(nodeId, _stream.Buffer, _stream.GetLength());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            _stream.Reset();
        }
    }
}