using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace HotKeyEvent
{

    /// <summary>
    /// Interaction logic for GlobalHotKey.xaml
    /// </summary>
    public partial class GlobalHotKey : Window
    {
        private HwndSource source;
        private HwndSourceHook hHook;
        private IntPtr hwnd;

        private int hotKeyId;
        private ushort modKey;
        private ushort virtualKey;

        public delegate void HotKeyDownEvent(int wparam);
        public static event HotKeyDownEvent HotKeyDown;

        // Modifier key for register / unregister global hotkey
        public static readonly ushort MOD_ALT = 0x0001;
        public static readonly ushort MOD_CONTROL = 0x0002;
        public static readonly ushort MOD_NOREPEAT = 0x4000;
        public static readonly ushort MOD_SHIFT = 0x0004;
        public static readonly ushort MOD_WIN = 0x0008;

        public GlobalHotKey(int id, ushort modifiers, ushort vk)
        {
            InitializeComponent();
            source = null;
            hotKeyId = id;
            modKey = modifiers;
            virtualKey = vk;
        }

        /// <summary>
        /// Start hotkey capture
        /// </summary>
        /// <returns>if the hotkey register success, return true, otherwise return false.</returns>
        public bool Start()
        {
            Show();
            return Set(hotKeyId, modKey, virtualKey);
        }

        /// <summary>
        /// Update registered hotkey
        /// </summary>
        /// <param name="id">hotkey id</param>
        /// <param name="modifiers">modifier key flags</param>
        /// <param name="vk">Virtual key (Win32)</param>
        /// <returns>if the hotkey update success, return true, otherwise return false.</returns>
        public bool Update(int id, ushort modifiers, ushort vk)
        {
            UnSet(hotKeyId);
            hotKeyId = id;
            modKey = modifiers;
            virtualKey = vk;
            return Set(id, modifiers, vk);
        }

        /// <summary>
        /// Stop hotkey capture
        /// </summary>
        public void Stop()
        {
            Close();
        }

        /// <summary>
        /// Get parent window handle (hwnd) and set callback
        /// </summary>
        /// <param name="e">Eventargs</param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            hwnd = new WindowInteropHelper(this).Handle;
            source = HwndSource.FromHwnd(hwnd);
            if (SetWindowProc(new HwndSourceHook(WindowProcess)))
            {
                Debug.WriteLine("[GlobalHotKey.OnSourceInitialized] The WndProc is prepared.");
            }
        }

        /// <summary>
        /// Close hook and callback
        /// </summary>
        /// <param name="e">Eventargs</param>
        protected override void OnClosed(EventArgs e)
        {
            source.RemoveHook(hHook);
            source = null;
            UnSet(hotKeyId);
            UnSetWindowProc();
            base.OnClosed(e);
        }

        /// <summary>
        /// Add callback and keep callback handle
        /// </summary>
        /// <param name="hook">Callback function wrapped by HwndSourceHook</param>
        /// <returns></returns>
        private bool SetWindowProc(HwndSourceHook hook)
        {
            if (source == null)
            {
                Debug.WriteLine("[GlobalHotKey.SetWindowProc] HwndSource is null.");
                return false;
            }

            source.AddHook(hook);
            hHook = hook;

            return true;
        }

        /// <summary>
        /// Remove callback
        /// </summary>
        private void UnSetWindowProc()
        {
            if (source == null)
            {
                Debug.WriteLine("[GlobalHotKey.UnSetWindowProc] HwndSource is null.");
                return;
            }

            source.RemoveHook(hHook);
            hHook = null;
        }

        /// <summary>
        /// Resister new hotkey
        /// </summary>
        /// <param name="id">Associate id for hotkey</param>
        /// <param name="modifiers">MOD_XXX members. It allows multiple selection</param>
        /// <param name="vk">Win32 virtual key</param>
        /// <returns>if register success, return true, otherwise return false.</returns>
        private bool Set(int id, ushort modifiers, ushort vk)
        {

            if (NativeMethods.RegisterHotKey(
                hwnd,
                id,
                modifiers,
                vk))
            {
                Debug.WriteLine("[HotKey.Set] Register Success");
                return true;
            }

            Debug.WriteLine($"Error : {Marshal.GetLastWin32Error():X08}");
            return false;
        }

        /// <summary>
        /// Unregister hotkey
        /// </summary>
        /// <param name="id">Associate id for Hotkey</param>
        private void UnSet(int id)
        {
            if (NativeMethods.UnregisterHotKey(hwnd, id))
            {
                Debug.WriteLine("[HotKey.UnSet] UnRegister Success");
            }
        }

        /// <summary>
        /// Native WndProc
        /// </summary>
        /// <param name="hwnd">Window handle</param>
        /// <param name="message">message No.</param>
        /// <param name="wParam">param 1</param>
        /// <param name="lParam">param 2</param>
        /// <param name="handled">if true, capture the hotkey</param>
        /// <returns>0</returns>
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
                HotKeyDown?.Invoke((int)wParam);
            }
            return IntPtr.Zero;
        }

    }
}
