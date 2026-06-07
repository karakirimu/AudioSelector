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
using System.Runtime.InteropServices;
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
        private AudioDeviceEnumerationEvent speakerEnumerationEvent;
        private AudioDeviceEnumerationEvent microphoneEnumerationEvent;
        private DeviceVolumeChangeEvent volumeChangeEvent;
        private GlobalHotKey speakerHotKey;
        private GlobalHotKey microphoneHotKey;
        private bool speakerHotKeyRegistered;
        private bool microphoneHotKeyRegistered;
        private AudioSelectorViewModel viewModel;
        private AppConfig appConfig;
        private ServiceProvider container;
        private DynamicResource dynamicResource;

        // Prevent multiple instances
        private MultiInstanceHandler multi;
        private bool isLaunched = false;
        private bool isStoreApp;

        public App()
        {
            Startup += (o, e) =>
            {
                isStoreApp = StartupStoreApp.IsStoreApp();
                if (isStoreApp)
                {
                    StoreCrashReporting.Register(this);
                }

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
                speakerHotKey = new GlobalHotKey();
                microphoneHotKey = new GlobalHotKey();
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
                speakerEnumerationEvent = new(AudioDeviceKind.Speaker);
                microphoneEnumerationEvent = new(AudioDeviceKind.Microphone);
                speakerEnumerationEvent.Start();
                microphoneEnumerationEvent.Start();
                speakerEnumerationEvent.Add += OnSpeakerDeviceAdd;
                speakerEnumerationEvent.Remove += OnSpeakerDeviceRemoved;
                microphoneEnumerationEvent.Add += OnMicrophoneDeviceAdd;
                microphoneEnumerationEvent.Remove += OnMicrophoneDeviceRemoved;

                // Audio device volume change event setup
                volumeChangeEvent = new();
                foreach (var device in speakerEnumerationEvent.Devices.Concat(microphoneEnumerationEvent.Devices))
                {
                    volumeChangeEvent.AddCallback(device.Id);
                }

                viewModel.SpeakerDevices = new ObservableCollection<MultiMediaDevice>(speakerEnumerationEvent.Devices);
                viewModel.MicrophoneDevices = new ObservableCollection<MultiMediaDevice>(microphoneEnumerationEvent.Devices);
                viewModel.CurrentDeviceKind = AudioDeviceKind.Speaker;
                viewModel.AppConfig = appConfig;
                viewModel.VolumeChangeEvent = volumeChangeEvent;

                Current.MainWindow = new MainWindow
                {
                    DataContext = viewModel
                };

                var service = new ServiceCollection();
                service.AddSingleton(viewModel);
                service.AddSingleton(Current.MainWindow);
                service.AddSingleton<Window>(Current.MainWindow);
                container = service.BuildServiceProvider();

                UpdateTheme(appConfig.Property);
                UpdateStartup(appConfig.Property);
                appConfig.UserConfigurationUpdate += OnUserConfigurationUpdate;
                isLaunched = true;

            };

            Exit += (o, e) =>
            {
                // Prevent multiple instances
                if (multi != null)
                {
                    multi.AnotherAppLaunched -= OnAnotherAppLaunched;
                    multi.Stop();
                }

                if(isLaunched == false) return;

                speakerHotKey.Close();
                microphoneHotKey.Close();
                foreach (var device in speakerEnumerationEvent.Devices.Concat(microphoneEnumerationEvent.Devices))
                {
                    RemoveVolumeCallback(device.Id);
                }

                speakerEnumerationEvent.Stop();
                microphoneEnumerationEvent.Stop();
                speakerEnumerationEvent.Add -= OnSpeakerDeviceAdd;
                speakerEnumerationEvent.Remove -= OnSpeakerDeviceRemoved;
                microphoneEnumerationEvent.Add -= OnMicrophoneDeviceAdd;
                microphoneEnumerationEvent.Remove -= OnMicrophoneDeviceRemoved;
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
                case AppConfigType.SpeakerHotKeyEnabled:
                case AppConfigType.SpeakerHotKey:
                case AppConfigType.SpeakerHotKeyId:
                case AppConfigType.MicrophoneHotKeyEnabled:
                case AppConfigType.MicrophoneHotKey:
                case AppConfigType.MicrophoneHotKeyId:
                case AppConfigType.TrayDoubleClickTarget:
                    UpdateLanguageAndHotKey(config);
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

        private bool UpdateHotKey(GlobalHotKey targetHotKey, ushort id, HotKey hotkey, bool enabled, bool registered, bool initialize)
        {
            ushort modifier = GetModifier(hotkey);
            Key key = (Key)Enum.Parse(typeof(Key), hotkey.VirtualKey);
            Keys formsKey = (Keys)KeyInterop.VirtualKeyFromKey(key);

            if(enabled == false)
            {
                if (registered)
                {
                    targetHotKey.Stop();
                }
                return false;
            }

            if (initialize || !registered)
            {
                if (!targetHotKey.Start(id, modifier, (ushort)formsKey))
                {
                    ShowHotKeyError();
                    return false;
                }
                return true;
            }

            if (!targetHotKey.Update(id, modifier, (ushort)formsKey))
            {
                ShowHotKeyError();
                return false;
            }

            return true;
        }

        private static ushort GetModifier(HotKey hotkey)
        {
            ushort modifier = 0;
            if (hotkey.Win)
            {
                modifier |= GlobalHotKey.MOD_WIN;
            }
            if (hotkey.Ctrl)
            {
                modifier |= GlobalHotKey.MOD_CONTROL;
            }
            if (hotkey.Alt)
            {
                modifier |= GlobalHotKey.MOD_ALT;
            }
            if (hotkey.Shift)
            {
                modifier |= GlobalHotKey.MOD_SHIFT;
            }

            return modifier;
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

            speakerHotKeyRegistered = UpdateHotKey(
                speakerHotKey,
                config.SpeakerHotkeyId,
                config.SpeakerHotkey,
                config.SpeakerHotkeyEnabled,
                speakerHotKeyRegistered,
                initialize);
            microphoneHotKeyRegistered = UpdateHotKey(
                microphoneHotKey,
                config.MicrophoneHotkeyId,
                config.MicrophoneHotkey,
                config.MicrophoneHotkeyEnabled,
                microphoneHotKeyRegistered,
                initialize);
            UpdateTaskbarToolTip(config);
        }

        private void UpdateTaskbarToolTip(AppConfigProperty config)
        {
            string speakerHotkey = GetHotKeyText(config.SpeakerHotkey, config.SpeakerHotkeyEnabled);
            string microphoneHotkey = GetHotKeyText(config.MicrophoneHotkey, config.MicrophoneHotkeyEnabled);
            string doubleClickTarget = GetDeviceKindText(ConvertTarget(config.TrayDoubleClickTarget));

            taskbarControl.Text = string.Format(
                GetResourceString("TaskbarToolTip"),
                speakerHotkey,
                microphoneHotkey,
                doubleClickTarget);
        }

        private static string GetHotKeyText(HotKey hotkey, bool enabled)
        {
            if (!enabled)
            {
                return GetResourceString("HotKeyDisabled");
            }

            List<string> keylist = [];
            if (hotkey.Win)
            {
                keylist.Add(AudioSelector.Properties.Resources.KeyWin);
            }
            if (hotkey.Ctrl)
            {
                keylist.Add(AudioSelector.Properties.Resources.KeyCtrl);
            }
            if (hotkey.Alt)
            {
                keylist.Add(AudioSelector.Properties.Resources.KeyAlt);
            }
            if (hotkey.Shift)
            {
                keylist.Add(AudioSelector.Properties.Resources.KeyShift);
            }
            keylist.Add(hotkey.VirtualKey);

            return string.Join("+", keylist);
        }

        private static string GetDeviceKindText(AudioDeviceKind kind)
        {
            return kind switch
            {
                AudioDeviceKind.Microphone => GetResourceString("SettingMicrophone"),
                AudioDeviceKind.Speaker or _ => GetResourceString("SettingSpeaker"),
            };
        }

        private static string GetResourceString(string name)
        {
            return AudioSelector.Properties.Resources.ResourceManager.GetString(name) ?? name;
        }

        private void UpdateStartup(AppConfigProperty config)
        {
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
            ShowSelectWindow(ConvertTarget(appConfig.Property.TrayDoubleClickTarget));
        }

        private void OnKeyChange(int param)
        {
            if (param == appConfig.Property.MicrophoneHotkeyId)
            {
                ShowSelectWindow(AudioDeviceKind.Microphone);
                return;
            }

            if (param == appConfig.Property.SpeakerHotkeyId)
            {
                ShowSelectWindow(AudioDeviceKind.Speaker);
            }
        }

        private static AudioDeviceKind ConvertTarget(TrayDoubleClickTarget target)
        {
            return target switch
            {
                TrayDoubleClickTarget.Microphone => AudioDeviceKind.Microphone,
                TrayDoubleClickTarget.Speaker or _ => AudioDeviceKind.Speaker,
            };
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
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(OnAnotherAppLaunched);
                return;
            }

            string exeName = System.IO.Path.GetFileNameWithoutExtension(Environment.ProcessPath);
            taskbarControl.ShowBalloonTip(3000,
                exeName,
                AudioSelector.Properties.Resources.DuplicateApp,
                ToolTipIcon.Info);
        }

        /// <summary>
        /// Show audio device selector window
        /// </summary>
        private void ShowSelectWindow(AudioDeviceKind kind)
        {
            IReadOnlyCollection<MultiMediaDevice> devices = GetDevices(kind);
            if (devices.Count == 0)
            {
                Debug.WriteLine($"[App.ShowSelectWindow] No device listed");
                return;
            }

            try
            {
                taskbarControl.Visible = false;
                viewModel.CurrentDeviceKind = kind;
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

        private IReadOnlyCollection<MultiMediaDevice> GetDevices(AudioDeviceKind kind)
        {
            return kind switch
            {
                AudioDeviceKind.Microphone => microphoneEnumerationEvent.Devices,
                AudioDeviceKind.Speaker or _ => speakerEnumerationEvent.Devices,
            };
        }

        private void OnSpeakerDeviceAdd(MultiMediaDevice device)
        {
            Debug.WriteLine("[App.OnSpeakerDeviceAdd]");
            volumeChangeEvent.AddCallback(device.Id);
            viewModel.SpeakerDevices.Add(device);
        }

        private void OnSpeakerDeviceRemoved(MultiMediaDevice device)
        {
            Debug.WriteLine("[App.OnSpeakerDeviceRemove]");
            RemoveVolumeCallback(device.Id);
            MultiMediaDevice foundDevice = viewModel.SpeakerDevices.FirstOrDefault(i => i.Id == device.Id);
            if (foundDevice != null && viewModel.SpeakerDevices.Remove(foundDevice))
            {
                Debug.WriteLine("[App.OnSpeakerDeviceRemove] Remove success");
            }
        }

        private void OnMicrophoneDeviceAdd(MultiMediaDevice device)
        {
            Debug.WriteLine("[App.OnMicrophoneDeviceAdd]");
            volumeChangeEvent.AddCallback(device.Id);
            viewModel.MicrophoneDevices.Add(device);
        }

        private void OnMicrophoneDeviceRemoved(MultiMediaDevice device)
        {
            Debug.WriteLine("[App.OnMicrophoneDeviceRemove]");
            RemoveVolumeCallback(device.Id);
            MultiMediaDevice foundDevice = viewModel.MicrophoneDevices.FirstOrDefault(i => i.Id == device.Id);
            if (foundDevice != null && viewModel.MicrophoneDevices.Remove(foundDevice))
            {
                Debug.WriteLine("[App.OnMicrophoneDeviceRemove] Remove success");
            }
        }

        private void RemoveVolumeCallback(string deviceId)
        {
            try
            {
                volumeChangeEvent.RemoveCallback(deviceId);
            }
            catch (COMException ex)
            {
                Debug.WriteLine($"[App.RemoveVolumeCallback] {ex.Message}");
            }
        }
    }
}
