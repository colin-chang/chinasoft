using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Com.ChinaSoft.Utility.Win32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public POINT(Point pt)
        {
            X = Convert.ToInt32(pt.X);
            Y = Convert.ToInt32(pt.Y);
        }
    };
}
