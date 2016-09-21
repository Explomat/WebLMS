using System;
using System.Runtime.InteropServices;

namespace WebLMS.Utils
{
    public static class Win32NativeMethods
    {
        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpFileName);
    }
}