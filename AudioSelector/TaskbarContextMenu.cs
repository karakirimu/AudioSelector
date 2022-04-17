using System;
using System.Windows.Forms;

namespace AudioSourceSelector
{
    /// <summary>
    /// Context Menu for NotifyIcon
    /// </summary>
    internal class TaskbarContextMenu
    {
        public ContextMenuStrip ContextMenu { get; private set; }


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

        public TaskbarContextMenu()
        {
            ContextMenu = new();
            ContextMenu.Items.Add(AddExitMenu());
        }

    }
}
