using AudioSelector.Properties;
using AudioSelector.Setting;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AudioSelector
{
    /// <summary>
    /// Context Menu for NotifyIcon
    /// </summary>
    internal class TaskbarContextMenu
    {
        private const int ContextMenuVerticalPadding = 0;
        private const int ContextMenuHorizontalPadding = 0;
        private const int ItemHorizontalPadding = 12;
        private const int ItemVerticalPadding = 0;
        private const int MenuItemHeight = 34;
        private const int MinimumMenuItemWidth = 150;

        private sealed class ThemeColors
        {
            public required Color Background { get; init; }
            public required Color Foreground { get; init; }
            public required Color HoverBackground { get; init; }
            public required Color SelectedBackground { get; init; }
            public required Color Border { get; init; }
        }

        private sealed class TaskbarContextMenuRenderer : ToolStripProfessionalRenderer
        {
            private readonly ThemeColors themeColors;

            public TaskbarContextMenuRenderer(ThemeColors themeColors) : base(new ProfessionalColorTable())
            {
                this.themeColors = themeColors;
                RoundedEdges = false;
            }

            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                using SolidBrush brush = new(themeColors.Background);
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
            }

            protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
            {
                using SolidBrush brush = new(themeColors.Background);
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                Color background = e.Item.Pressed
                    ? themeColors.SelectedBackground
                    : e.Item.Selected
                        ? themeColors.HoverBackground
                        : themeColors.Background;
                using SolidBrush brush = new(background);
                e.Graphics.FillRectangle(brush, new Rectangle(Point.Empty, e.Item.Size));
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.TextColor = themeColors.Foreground;
                base.OnRenderItemText(e);
            }

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                using Pen pen = new(themeColors.Border);
                Rectangle bounds = new(System.Drawing.Point.Empty, e.ToolStrip.Size);
                bounds.Width -= 1;
                bounds.Height -= 1;
                e.Graphics.DrawRectangle(pen, bounds);
            }
        }

        public ContextMenuStrip ContextMenu { get; private set; }
        private readonly IAppConfig appConfig;
        private readonly ToolStripMenuItem settingMenu;
        private readonly ToolStripMenuItem exitMenu;
        private SystemTheme theme;
        private Settings settingsWindow { get; set; }

        /// <summary>
        /// Menu item for application setting
        /// </summary>
        /// <returns></returns>
        private ToolStripMenuItem AddSettingMenu()
        {
            ToolStripMenuItem item = new(Properties.Resources.TaskBarMenuSetting);
            item.Click += OpenSettingWindow;
            ApplyItemTheme(item);

            return item;
        }

        /// <summary>
        /// Menu item for application exit
        /// </summary>
        /// <returns></returns>
        private ToolStripMenuItem AddExitMenu()
        {
            ToolStripMenuItem item = new(Properties.Resources.TaskbarMenuExit);
            item.Click += AppShutdown;
            ApplyItemTheme(item);

            return item;
        }

        private void ApplyItemTheme(ToolStripMenuItem item)
        {
            ThemeColors colors = GetThemeColors();
            item.AutoSize = false;
            item.BackColor = colors.Background;
            item.ForeColor = colors.Foreground;
            item.Padding = new Padding(ItemHorizontalPadding, ItemVerticalPadding, ItemHorizontalPadding, ItemVerticalPadding);
            item.Margin = Padding.Empty;
            item.TextAlign = ContentAlignment.MiddleLeft;
        }

        private void ApplyTheme()
        {
            ThemeColors colors = GetThemeColors();
            Size menuSize = CalculateMenuSize();
            ContextMenu.BackColor = colors.Background;
            ContextMenu.ForeColor = colors.Foreground;
            ContextMenu.AutoSize = false;
            ContextMenu.Padding = new Padding(ContextMenuHorizontalPadding, ContextMenuVerticalPadding, ContextMenuHorizontalPadding, ContextMenuVerticalPadding);
            ContextMenu.Margin = Padding.Empty;
            ContextMenu.ShowImageMargin = false;
            ContextMenu.ShowCheckMargin = false;
            ContextMenu.CanOverflow = false;
            ContextMenu.Renderer = new TaskbarContextMenuRenderer(colors);

            foreach (ToolStripItem item in ContextMenu.Items)
            {
                item.BackColor = colors.Background;
                item.ForeColor = colors.Foreground;

                if (item is ToolStripMenuItem menuItem)
                {
                    menuItem.AutoSize = false;
                    menuItem.Size = new Size(
                        menuSize.Width - (ContextMenuHorizontalPadding * 2) - 2,
                        MenuItemHeight);
                    menuItem.Padding = new Padding(ItemHorizontalPadding, ItemVerticalPadding, ItemHorizontalPadding, ItemVerticalPadding);
                }
            }

            ContextMenu.Size = menuSize;
            ContextMenu.PerformLayout();
            Size preferredSize = ContextMenu.GetPreferredSize(Size.Empty);
            ContextMenu.Size = new Size(menuSize.Width, Math.Max(menuSize.Height, preferredSize.Height));
        }

        private Size CalculateMenuSize()
        {
            int itemWidth = MinimumMenuItemWidth;
            int totalHeight = (ContextMenuVerticalPadding * 2) + 2;

            foreach (ToolStripItem item in ContextMenu.Items)
            {
                if (item is not ToolStripMenuItem menuItem)
                {
                    continue;
                }

                Size textSize = TextRenderer.MeasureText(menuItem.Text, menuItem.Font, Size.Empty, TextFormatFlags.SingleLine);
                int candidateWidth = textSize.Width + menuItem.Padding.Horizontal;
                itemWidth = Math.Max(itemWidth, candidateWidth);
                totalHeight += MenuItemHeight;
            }

            return new Size(itemWidth + (ContextMenuHorizontalPadding * 2) + 2, totalHeight);
        }

        private ThemeColors GetThemeColors()
        {
            System.Windows.ResourceDictionary dictionary = DynamicResource.LoadThemeDictionary(theme);

            return new ThemeColors
            {
                Background = GetColor(dictionary, "ListBackgroundColor"),
                Foreground = GetColor(dictionary, "TextBlockForegroundColor"),
                HoverBackground = GetColor(dictionary, "MouseOverColor"),
                SelectedBackground = GetColor(dictionary, "SelectedColor"),
                Border = GetColor(dictionary, "ComboboxBorderColor")
            };
        }

        private static Color GetColor(System.Windows.ResourceDictionary dictionary, string key)
        {
            object value = dictionary[key];

            return value switch
            {
                System.Windows.Media.Color color => Color.FromArgb(color.A, color.R, color.G, color.B),
                System.Windows.Media.SolidColorBrush brush => Color.FromArgb(brush.Color.A, brush.Color.R, brush.Color.G, brush.Color.B),
                _ => throw new InvalidOperationException($"Theme resource '{key}' is not a color.")
            };
        }

        private void OnContextMenuOpening(object sender, CancelEventArgs e)
        {
            ApplyTheme();
        }

        public void Update(SystemTheme theme)
        {
            this.theme = theme;
            settingMenu.Text = Properties.Resources.TaskBarMenuSetting;
            exitMenu.Text = Properties.Resources.TaskbarMenuExit;
            ApplyTheme();
        }

        private void OpenSettingWindow(object sender, EventArgs e)
        {
            if (settingsWindow.IsVisible == false)
            {
                settingsWindow.Show();
            }
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

        public TaskbarContextMenu(IAppConfig config, SystemTheme theme)
        {
            appConfig = config;
            this.theme = theme;
            settingsWindow = new Settings(appConfig);
            ContextMenu = new();
            ContextMenu.Opening += OnContextMenuOpening;
            settingMenu = AddSettingMenu();
            exitMenu = AddExitMenu();
            ContextMenu.Items.Add(settingMenu);
            ContextMenu.Items.Add(exitMenu);
            ApplyTheme();
        }

    }
}
