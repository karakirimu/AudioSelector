using System;
using System.Drawing;
using System.Windows.Forms;

namespace AudioSelector
{
    /// <summary>
    /// The wrapper for NotifyIcon
    /// </summary>
    public class TaskbarIconControl : IDisposable
    {
        private readonly NotifyIcon notifyIcon;
        private bool disposedValue;

        public string Text
        {
            get => notifyIcon.Text;
            set => notifyIcon.Text = value;
        }

        public Icon Icon
        {
            get => notifyIcon.Icon;
            set => notifyIcon.Icon = value;
        }

        public bool Visible
        {
            get => notifyIcon.Visible;
            set => notifyIcon.Visible = value;
        }

        public ContextMenuStrip ContextMenuStrip
        {
            get => notifyIcon.ContextMenuStrip;
            set => notifyIcon.ContextMenuStrip = value;
        }

        public TaskbarIconControl()
        {
            notifyIcon = new();
        }

        public event EventHandler Click
        {
            add
            {
                notifyIcon.Click += value;
            }
            remove
            {
                notifyIcon.Click -= value;
            }
        }

        public event EventHandler DoubleClick
        {
            add
            {
                notifyIcon.DoubleClick += value;
            }
            remove
            {
                notifyIcon.DoubleClick -= value;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    notifyIcon.Dispose();
                }

                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
