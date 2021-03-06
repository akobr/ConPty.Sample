using System;
using System.Runtime.InteropServices;

namespace ConPty.Sample.ConsoleApi.Interop.Definitions
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ProcessInfo
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public int dwProcessId;
        public int dwThreadId;
    }
}
