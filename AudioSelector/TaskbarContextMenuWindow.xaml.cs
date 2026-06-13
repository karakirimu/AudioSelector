using System;
using AudioSelector.Setting;
using System.Windows;
using System.Windows.Input;

namespace AudioSelector
{
    /// <summary>
    /// Interaction logic for TaskbarContextMenuWindow.xaml
    /// </summary>
    public partial class TaskbarContextMenuWindow : Window
    {
        private Point lastCursorPosition;
        private Rect lastWorkingArea;

        public event EventHandler SettingsClicked;
        public event EventHandler ExitClicked;

        public TaskbarContextMenuWindow()
        {
            InitializeComponent();

            Deactivated += (o, e) =>
            {
                HideMenu();
            };

            PreviewKeyDown += OnPreviewKeyDown;
        }

        private Size GetMenuSize()
        {
            RootBorder.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return RootBorder.DesiredSize;
        }

        private void ApplyPlacement()
        {
            Size menuSize = GetMenuSize();

            double left = lastCursorPosition.X;
            double top = lastCursorPosition.Y;

            if (left + menuSize.Width > lastWorkingArea.Right)
            {
                left = lastCursorPosition.X - menuSize.Width;
            }

            if (top + menuSize.Height > lastWorkingArea.Bottom)
            {
                top = lastCursorPosition.Y - menuSize.Height;
            }

            Left = Math.Max(lastWorkingArea.Left, Math.Min(left, lastWorkingArea.Right - menuSize.Width));
            Top = Math.Max(lastWorkingArea.Top, Math.Min(top, lastWorkingArea.Bottom - menuSize.Height));
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape)
            {
                return;
            }

            HideMenu();
            e.Handled = true;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            HideMenu();
            SettingsClicked?.Invoke(sender, EventArgs.Empty);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            HideMenu();
            ExitClicked?.Invoke(sender, EventArgs.Empty);
        }

        public void ShowMenu(Point cursorPosition, Rect workingArea)
        {
            lastCursorPosition = cursorPosition;
            lastWorkingArea = workingArea;
            ApplyPlacement();

            if (IsVisible == false)
            {
                Show();
            }
            else
            {
                _ = Activate();
            }

            ApplyPlacement();
            _ = Focus();
        }

        public void HideMenu()
        {
            if (IsVisible)
            {
                Keyboard.ClearFocus();
                Hide();
            }
        }

        public void UpdateMenu(SystemTheme theme)
        {
            _ = theme;

            if (IsVisible == false)
            {
                return;
            }

            ApplyPlacement();
        }
    }
}
