using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AudioSelector
{
    internal class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromPoint(Point pt, uint dwFlags);

        [DllImport("shcore.dll")]
        public static extern int GetDpiForMonitor(IntPtr hmonitor, int dpiType, out uint dpiX, out uint dpiY);

        public const int MDT_EFFECTIVE_DPI = 0;
        public const uint MONITOR_DEFAULTTONEAREST = 2;
    }
}
