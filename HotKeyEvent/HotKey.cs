using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace HotKeyEvent
{
    public class HotKey
    {
        private readonly MessageWindow messageWindow;
        private int idCount;
        private IntPtr hwnd = IntPtr.Zero;

        public static readonly ushort MOD_ALT = 0x0001;
        public static readonly ushort MOD_CONTROL = 0x0002;
        public static readonly ushort MOD_NOREPEAT = 0x4000;
        public static readonly ushort MOD_SHIFT = 0x0004;
        public static readonly ushort MOD_WIN = 0x0008;

        public delegate void HotKeyDownEvent(int id);
        public static event HotKeyDownEvent KeyDown;

        public HotKey()
        {
            messageWindow = new();
            hwnd = new WindowInteropHelper(messageWindow).Handle;
        }

        public void Set(int id, ushort modifiers, ushort vk)
        {

            if (NativeMethods.RegisterHotKey(
                hwnd,
                id,
                modifiers,
                vk))
            {
                idCount++;
                if (idCount == 1)
                {
                    Debug.WriteLine("[HotKey.Set] Register Success");
                    messageWindow.Show();
                    messageWindow.SetWindwProc(new HwndSourceHook(WindowProcess));
                }
                return;
            }

            Debug.WriteLine($"Error : {Marshal.GetLastWin32Error():X08}");
        }

        public void UnSet(int id)
        {
            if (NativeMethods.UnregisterHotKey(hwnd, id))
            {
                idCount--;
                if (idCount == 0)
                {
                    Debug.WriteLine("[HotKey.Set] Register Success");
                    messageWindow.UnSetWindowProc();
                    messageWindow.Hide();
                }
            }
        }

        private static IntPtr WindowProcess(
            IntPtr hwnd,
            int message,
            IntPtr wParam,
            IntPtr lParam,
            ref bool handled)
        {
            if (message == NativeMethods.WM_HOTKEY)
            {
                Debug.WriteLine($"Hotkey {wParam}");
                handled = true;
                KeyDown?.Invoke((int)wParam);
            }
            return IntPtr.Zero;
        }

    }
}
