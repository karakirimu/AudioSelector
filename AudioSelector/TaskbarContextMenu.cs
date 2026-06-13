using AudioSelector.Setting;
using System;
using System.Windows;

namespace AudioSelector
{
    /// <summary>
    /// Context Menu for NotifyIcon
    /// </summary>
    internal class TaskbarContextMenu
    {
        private readonly Settings settingsWindow;
        private readonly TaskbarContextMenuWindow menuWindow;

        /// <summary>
        /// WPF tray menu item for application setting
        /// </summary>
        private void OpenSettingWindow(object sender, EventArgs e)
        {
            if (settingsWindow.IsVisible == false)
            {
                settingsWindow.Show();
            }

            _ = settingsWindow.Activate();
        }

        /// <summary>
        /// Mouse click event when clicked by exit menu
        /// </summary>
        private void AppShutdown(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
            Application.Current.Shutdown(0);
        }

        private Rect GetWorkingArea(System.Drawing.Point cursorPosition, double dpiScale)
        {
            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromPoint(cursorPosition);

            return new Rect(
                screen.WorkingArea.X * dpiScale,
                screen.WorkingArea.Y * dpiScale,
                screen.WorkingArea.Width * dpiScale,
                screen.WorkingArea.Height * dpiScale);
        }

        private static double GetDpiScale(System.Drawing.Point cursorPosition)
        {
            IntPtr monitor = NativeMethods.MonitorFromPoint(cursorPosition, NativeMethods.MONITOR_DEFAULTTONEAREST);
            int result = NativeMethods.GetDpiForMonitor(monitor, NativeMethods.MDT_EFFECTIVE_DPI, out uint dpiX, out _);
            if (result != 0 || dpiX == 0)
            {
                return 1;
            }

            return 96d / dpiX;
        }

        private static System.Windows.Point ConvertToDipPoint(System.Drawing.Point cursorPosition, double dpiScale)
        {
            return new System.Windows.Point(cursorPosition.X * dpiScale, cursorPosition.Y * dpiScale);
        }

        public void ShowAtCursor(System.Drawing.Point cursorPosition)
        {
            double dpiScale = GetDpiScale(cursorPosition);
            Rect workingArea = GetWorkingArea(cursorPosition, dpiScale);
            System.Windows.Point menuPosition = ConvertToDipPoint(cursorPosition, dpiScale);

            menuWindow.ShowMenu(menuPosition, workingArea);
        }

        public void Close()
        {
            menuWindow.HideMenu();
        }

        public void Update(SystemTheme theme)
        {
            menuWindow.UpdateMenu(theme);
        }

        public TaskbarContextMenu(IAppConfig config, SystemTheme theme)
        {
            settingsWindow = new Settings(config);
            menuWindow = new TaskbarContextMenuWindow();
            menuWindow.SettingsClicked += OpenSettingWindow;
            menuWindow.ExitClicked += AppShutdown;
            menuWindow.UpdateMenu(theme);
        }
    }
}
