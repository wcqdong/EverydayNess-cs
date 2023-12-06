namespace ConnService.Netty.Kcp.KcpCore
{
    public class KcpAgent : IDisposable
    {
        private IntPtr _kcp;
        public bool Disposed { get; private set; }

        public uint Conv => Kcp.ikcp_getconv(_kcp);
        public int WaitSnd => Kcp.ikcp_waitsnd(_kcp);

        /** Kcp 报文头大小 */
        public const int KcpOverHeadSize = 24;

        /** Kcp 最小接收窗口 */
        public const int KcpMinRcvWndSize = 128;

        public static void SetAllocator(KcpMalloc malloc, KcpFree free)
        {
            Kcp.ikcp_allocator(malloc, free);
        }

        public KcpAgent(uint conv, KcpOutput output)
        {
            Disposed = false;
            _kcp = Kcp.ikcp_create(conv, (IntPtr) conv);

            Kcp.ikcp_setoutput(_kcp, output);
        }

        public void Dispose()
        {
            Disposed = true;

            Kcp.ikcp_release(_kcp);

            _kcp = IntPtr.Zero;
        }

        public uint Check(uint current)
        {
            return Kcp.ikcp_check(_kcp, current);
        }

        public void Flush()
        {
            Kcp.ikcp_flush(_kcp);
        }

        public int Input(ArraySegment<byte> bytes)
        {
            unsafe
            {
                fixed (void* ptr = &bytes.Array![bytes.Offset])
                {
                    return Kcp.ikcp_input(_kcp, (IntPtr)ptr, bytes.Count);
                }
            }
        }

        public int NoDelay(int noDelay, int interval, int resend, int nc)
        {
            return Kcp.ikcp_nodelay(_kcp, noDelay, interval, resend, nc);
        }

        public int PeekSize()
        {
            return Kcp.ikcp_peeksize(_kcp);
        }

        public int Recv(ArraySegment<byte> buffer)
        {
            unsafe
            {
                fixed (void* ptr = &buffer.Array![buffer.Offset])
                {
                    return Kcp.ikcp_recv(_kcp, (IntPtr)ptr, buffer.Count);
                }
            }
        }

        public int Send(ArraySegment<byte> buffer)
        {
            unsafe
            {
                fixed (byte* ptr = &buffer.Array![buffer.Offset])
                {
                    return Kcp.ikcp_send(_kcp, (IntPtr)ptr, buffer.Count);
                }
            }
        }

        public int SetMtu(int mtu)
        {
            return Kcp.ikcp_setmtu(_kcp, mtu);
        }

        public void Update(uint current)
        {
            Kcp.ikcp_update(_kcp, current);
        }

        public int WndSize(int sndWnd, int rcvWnd)
        {
            return Kcp.ikcp_wndsize(_kcp, sndWnd, rcvWnd);
        }

        public void SetStream(bool stream)
        {
            Kcp.ikcp_setstream(_kcp, stream ? 1 : 0);
        }

        public void SetMinRto(int minRto)
        {
            Kcp.ikcp_setminrto(_kcp, minRto);
        }
    }
}