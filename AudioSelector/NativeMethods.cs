using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AudioSelector
{
    internal partial class NativeMethods
    {
        [LibraryImport("user32.dll", SetLastError = true)]
        public static partial IntPtr MonitorFromPoint(Point pt, uint dwFlags);

        [LibraryImport("shcore.dll", SetLastError = true)]
        public static partial int GetDpiForMonitor(IntPtr hmonitor, int dpiType, out uint dpiX, out uint dpiY);

        public const int MDT_EFFECTIVE_DPI = 0;
        public const uint MONITOR_DEFAULTTONEAREST = 2;

        [LibraryImport("kernel32.dll", SetLastError = true)]
        public static partial int GetUserDefaultLCID();

    }
}
