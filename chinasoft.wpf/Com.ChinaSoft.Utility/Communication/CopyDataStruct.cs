using System;
using System.Runtime.InteropServices;

namespace Com.ChinaSoft.Utility.Communication
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CopyDataStruct
    {
        public IntPtr dwData;

        public int cbData;//字符串长度

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpData;//字符串
    }
}
