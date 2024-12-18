using AudioSelector.AudioDevice;
using AudioSelector.Properties;
using AudioSelector.Setting;
using AudioTools;
using HotKeyEvent;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace AudioSelector
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private TaskbarIconControl taskbarControl;
        private TaskbarContextMenu contextMenu;
        private AudioDeviceEnumerationEvent enumerationEvent;
        private DeviceVolumeChangeEvent volumeChangeEvent;
        private GlobalHotKey hotKey;
        private AudioSelectorViewModel viewModel;
        private AppConfig appConfig;
        private ServiceProvider container;
        private DynamicResource dynamicResource;

        // Prevent multiple instances
        private MultiInstanceHandler multi;
        private bool isLaunched = false;

        public App()
        {
            Startup += (o, e) =>
            {
                // Prevent multiple instances
                multi = new MultiInstanceHandler();
                if (!multi.Start())
                {
                    Current.Shutdown();
                    return;
                }

                appConfig = new AppConfig();
                viewModel = new AudioSelectorViewModel();
                dynamicResource = new DynamicResource();
                hotKey = new GlobalHotKey();
                GlobalHotKey.HotKeyDown += OnKeyChange;

                // Load json
                appConfig.Load();

                // Set system theme.
                InitializeTaskbarIcon();

                // Set language and hotkey
                UpdateLanguageAndHotKey(appConfig.Property, true);

                // Add DoubleClick event to taskbar icon.
                taskbarControl.DoubleClick += OnTaskIconDoubleClick;

                // Add context menu to taskbar icon.
                contextMenu = new(appConfig);
                taskbarControl.ContextMenuStrip = contextMenu.ContextMenu;

                // Audio device enumeration event setup
                enumerationEvent = new();
                enumerationEvent.Start();
                enumerationEvent.Add += OnDeviceAdd;
                enumerationEvent.Remove += OnDeviceRemoved;

                // Audio device volume change event setup
                volumeChangeEvent = new();
                foreach (var device in enumerationEvent.Devices)
                {
                    volumeChangeEvent.AddCallback(device.Id);
                }

                viewModel.Devices = new ObservableCollection<MultiMediaDevice>(enumerationEvent.Devices);
                viewModel.AppConfig = appConfig;
                viewModel.VolumeChangeEvent = volumeChangeEvent;

                Current.MainWindow = new MainWindow
                {
                    DataContext = viewModel
                };

                var service = new ServiceCollection();
                service.AddSingleton(viewModel);
                service.AddSingleton(MainWindow);
                container = service.BuildServiceProvider();

                UpdateTheme(appConfig.Property);
                UpdateStartup(appConfig.Property);
                appConfig.UserConfigurationUpdate += OnUserConfigurationUpdate;
                isLaunched = true;

            };

            Exit += (o, e) =>
            {
                // Prevent multiple instances
                multi.AnotherAppLaunched -= OnAnotherAppLaunched;
                multi.Stop();

                if(isLaunched == false) return;

                hotKey.Close();
                foreach (var device in enumerationEvent.Devices)
                {
                    volumeChangeEvent.RemoveCallback(device.Id);
                }

                enumerationEvent.Stop();
                enumerationEvent.Add -= OnDeviceAdd;
                enumerationEvent.Remove -= OnDeviceRemoved;
            };

        }

        private void OnUserConfigurationUpdate(AppConfigType type, AppConfigProperty config)
        {
            switch (type)
            {
                case AppConfigType.Theme:
                    UpdateTheme(config);
                    break;
                case AppConfigType.Language:
                case AppConfigType.HotKeyEnabled:
                    UpdateLanguageAndHotKey(config);
                    break;
                case AppConfigType.HotKey:
                    {
                        UpdateHotKey(config, false);
                        break;
                    }
                case AppConfigType.HotKeyId:
                    break;
                case AppConfigType.Startup:
                    UpdateStartup(config);
                    break;
            }
        }

        private void InitializeTaskbarIcon()
        {
            // Taskbar icon is always system theme.
            SystemTheme theme = SystemRegistry.GetCurrentTheme();
            System.Drawing.Icon taskbarIcon
            = theme switch
            {
                SystemTheme.Dark => AudioSelector.Properties.Resources.appicon_white,
                SystemTheme.Light or SystemTheme.System => AudioSelector.Properties.Resources.appicon_black,
                _ => AudioSelector.Properties.Resources.appicon_black,
            };

            taskbarControl = new()
            {
                Icon = taskbarIcon,
                Visible = true
            };

            multi.AnotherAppLaunched += OnAnotherAppLaunched;
        }

        private void UpdateHotKey(AppConfigProperty config, bool initialize)
        {
            List<string> keylist = [];
            ushort modifier = 0;
            if (config.Hotkey.Win)
            {
                modifier |= GlobalHotKey.MOD_WIN;
                keylist.Add(AudioSelector.Properties.Resources.KeyWin);
            }
            if (config.Hotkey.Ctrl)
            {
                modifier |= GlobalHotKey.MOD_CONTROL;
                keylist.Add(AudioSelector.Properties.Resources.KeyCtrl);
            }
            if (config.Hotkey.Alt)
            {
                modifier |= GlobalHotKey.MOD_ALT;
                keylist.Add(AudioSelector.Properties.Resources.KeyAlt);
            }
            if (config.Hotkey.Shift)
            {
                modifier |= GlobalHotKey.MOD_SHIFT;
                keylist.Add(AudioSelector.Properties.Resources.KeyShift);
            }
            keylist.Add(config.Hotkey.VirtualKey);

            string hotkeys = string.Join("+", keylist);
            taskbarControl.Text = string.Format(AudioSelector.Properties.Resources.TaskbarToolTip, hotkeys);

            Key key = (Key)Enum.Parse(typeof(Key), config.Hotkey.VirtualKey);
            Keys formsKey = (Keys)KeyInterop.VirtualKeyFromKey(key);

            if (initialize && config.Hotkey_enabled)
            {
                if (!hotKey.Start(config.Hotkey_id, modifier, (ushort)formsKey))
                {
                    ShowHotKeyError();
                }
                return;
            }

            if(config.Hotkey_enabled == false)
            {
                hotKey.Stop();
                return;
            }

            if (!hotKey.Update(config.Hotkey_id, modifier, (ushort)formsKey))
            {
                ShowHotKeyError();
            }
        }

        private void UpdateTheme(AppConfigProperty config)
        {
            dynamicResource.UpdateTheme(config.Theme);
        }

        private void UpdateLanguageAndHotKey(AppConfigProperty config, bool initialize = false)
        {
            var code = LanguageConverter.GetSupportedLanguageCode(config.Language);
            CultureInfo culture = new(code);
            System.Windows.Forms.Application.CurrentCulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            // Update context menu language
            contextMenu = new(appConfig);
            taskbarControl.ContextMenuStrip = contextMenu.ContextMenu;
            dynamicResource.UpdateLanguage(code);

            // Update hotkey tooltip language
            if (config.Hotkey_enabled)
            {
                UpdateHotKey(config, initialize);
                return;
            }

            UpdateHotKey(config, initialize);
            taskbarControl.Text = AudioSelector.Properties.Resources.TaskbarToolTipNoHotKey;
        }

        private static void UpdateStartup(AppConfigProperty config)
        {
            bool isStoreApp = StartupStoreApp.IsStoreApp();

            if (isStoreApp)
            {
                if (config.Startup)
                {
                    Task.Run(() => StartupStoreApp.EnableStartupTask());
                }
                else
                {
                    Task.Run(() => StartupStoreApp.DisableStartupTask());
                }
                return;
            }

            // for portable app

            bool registered = SystemRegistry.HasStartupEntry();
            if (config.Startup)
            {
                if (!registered)
                {
                    SystemRegistry.RegisterStartup();
                }
                return;
            }

            if (registered)
            {
                SystemRegistry.UnregisterStartup();
            }
        }

        private void OnTaskIconDoubleClick(object sender, EventArgs e)
        {
            ShowSelectWindow();
        }

        private void OnKeyChange(int param)
        {
            ShowSelectWindow();
        }

        private static void ShowHotKeyError()
        {
            string exeName = System.IO.Path.GetFileNameWithoutExtension(Environment.ProcessPath);
            _ = System.Windows.MessageBox.Show(
                AudioSelector.Properties.Resources.HotKeyRegistrationError,
                exeName,
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        private void OnAnotherAppLaunched()
        {
            string exeName = System.IO.Path.GetFileNameWithoutExtension(Environment.ProcessPath);
            taskbarControl.ShowBalloonTip(3000,
                exeName,
                AudioSelector.Properties.Resources.DuplicateApp,
                ToolTipIcon.Info);
        }

        /// <summary>
        /// Show audio device selector window
        /// </summary>
        private void ShowSelectWindow()
        {
            if (enumerationEvent.Devices.Count == 0)
            {
                Debug.WriteLine($"[App.ShowSelectWindow] No device listed");
                return;
            }

            try
            {
                taskbarControl.Visible = false;
                Window window = container.GetRequiredService<Window>();

                // Display the window at the cursor position.
                var cursorPosition = System.Windows.Forms.Cursor.Position;
                Debug.WriteLine($"Cursor Position: X = {cursorPosition.X}, Y = {cursorPosition.Y}");
                Screen screen = Screen.FromPoint(cursorPosition);

                IntPtr hMonitor = NativeMethods.MonitorFromPoint(cursorPosition, NativeMethods.MONITOR_DEFAULTTONEAREST);
                int result = NativeMethods.GetDpiForMonitor(hMonitor, NativeMethods.MDT_EFFECTIVE_DPI, out uint dpiX, out uint dpiY);
                if (result == 0) // S_OK
                {
                    Debug.WriteLine($"DPI: {dpiX} x {dpiY}");
                    window.Left = (screen.WorkingArea.X + (screen.WorkingArea.Width / 2)) * ((double)96 / dpiX) - (window.ActualWidth / 2);
                    window.Top = (screen.WorkingArea.Y + (screen.WorkingArea.Height / 2)) * ((double)96 / dpiY) - (window.ActualHeight / 2);
                    Debug.WriteLine($"Window Position: X = {window.Left}, Y = {window.Top}");
                }

                window.Show();
                if (window.Activate())
                {
                    Debug.WriteLine($"[App.ShowSelectWindow] Activate successful");
                }
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine($"{ex.Message}");
            }
            finally
            {
                taskbarControl.Visible = true;
            }
        }

        private void OnDeviceAdd(MultiMediaDevice device)
        {
            Debug.WriteLine("[App.OnDeviceAdd]");
            viewModel.Devices.Add(device);
        }

        private void OnDeviceRemoved(MultiMediaDevice device)
        {
            Debug.WriteLine("[App.OnDeviceRemove]");
            if (viewModel.Devices.Remove(viewModel.Devices.Where(i => i.Id == device.Id).Single()))
            {
                Debug.WriteLine("[App.OnDeviceRemove] Remove success");
            }
        }
    }
}
