using System;
using System.Runtime.InteropServices;

namespace HotKeyEvent
{
    internal class NativeMethods
    {
        public static readonly int WM_HOTKEY = 0x0312;

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, ushort modifiers, ushort vk);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}
