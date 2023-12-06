using System.Runtime.InteropServices;

namespace ConnService.Netty.Kcp.KcpCore
{
    public delegate int KcpOutput(IntPtr buf, int len, IntPtr kcp, IntPtr user);

    public delegate IntPtr KcpMalloc(int size);

    public delegate void KcpFree(IntPtr mem);

    internal static class Kcp
    {
        private const string KcpDll = "Netty/Kcp/KcpCore/kcp";
        // 以BootStrap为目录起点
        // private const string KcpDll = "../../Services/ConnService/Netty/Kcp/KcpCore/kcp";

        [DllImport(KcpDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ikcp_create(uint conv, IntPtr user);

        [DllImport(KcpDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ikcp_release(IntPtr kcp);

        [DllImport(KcpDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ikcp_setoutput(IntPtr kcp, KcpOutput output);

        [DllImport(KcpDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ikcp_recv(IntPtr kcp, IntPtr buffer, int len);

        [DllImport(KcpDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ikcp_send(IntPtr kcp, IntPtr buffer, int len);

        [DllImport(KcpDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ikcp_update(IntPtr kcp, uint current);

        [DllImport(KcpDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ikcp_check(IntPtr kcp, uint current);

        [DllImport(KcpDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ikcp_input(IntPtr kcp, IntPtr data, int size);

        [DllImport(KcpDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ikcp_flush(IntPtr kcp);

        [DllImport(KcpDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ikcp_peeksize(IntPtr kcp);

        [DllImport(KcpDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ikcp_setmtu(IntPtr kcp, int mtu);

        [DllImport(KcpDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ikcp_wndsize(IntPtr kcp, int sndwnd, int rcvwnd);

        [DllImport(KcpDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ikcp_waitsnd(IntPtr kcp);

        [DllImport(KcpDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ikcp_nodelay(IntPtr kcp, int nodelay, int interval, int resend, int nc);

        // setup allocator
        [DllImport(KcpDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ikcp_allocator(KcpMalloc malloc, KcpFree free);

        [DllImport(KcpDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ikcp_getconv(IntPtr kcp);

        [DllImport(KcpDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ikcp_setstream(IntPtr kcp, int mode);

        [DllImport(KcpDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ikcp_setminrto(IntPtr kcp, int min_rto);
    }
}