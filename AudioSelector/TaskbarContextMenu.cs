using AudioSelector.Setting;
using System;
using System.Windows.Forms;

namespace AudioSelector
{
    /// <summary>
    /// Context Menu for NotifyIcon
    /// </summary>
    internal class TaskbarContextMenu
    {
        public ContextMenuStrip ContextMenu { get; private set; }
        private IAppConfig appConfig;

        /// <summary>
        /// Menu item for application setting
        /// </summary>
        /// <returns></returns>
        private ToolStripMenuItem AddSettingMenu()
        {
            ToolStripMenuItem item = new("Setting");
            item.Click += OpenSettingWindow;

            return item;
        }

        /// <summary>
        /// Menu item for application exit
        /// </summary>
        /// <returns></returns>
        private ToolStripMenuItem AddExitMenu()
        {
            ToolStripMenuItem item = new("Exit");
            item.Click += AppShutdown;

            return item;
        }

        private void OpenSettingWindow(object sender, EventArgs e)
        {
            new Settings(appConfig).ShowDialog();
        }

        /// <summary>
        /// Mouse click event when clicked by AddExitMenu function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppShutdown(object sender, EventArgs e)
        {
            Application.Exit();
            System.Windows.Application.Current.Shutdown(0);
        }

        public TaskbarContextMenu(IAppConfig config)
        {
            ContextMenu = new();
            ContextMenu.Items.Add(AddSettingMenu());
            ContextMenu.Items.Add(AddExitMenu());
            appConfig = config;
        }

    }
}
