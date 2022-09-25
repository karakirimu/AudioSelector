using AudioSelector;
using AudioSourceSelector.AudioDevice;
using AudioTools;
using HotKeyEvent;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace AudioSourceSelector
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIconControl taskbarControl;
        private TaskbarContextMenu contextMenu;
        private AudioDeviceEnumerationEvent enumerationEvent;
        private GlobalHotKey hotKey;
        private AudioSelectorViewModel viewModel;
        private ServiceProvider container;

        /// <summary>
        /// Hotkey ID
        /// </summary>
        private const int ID = 0x2652;

        public App()
        {
            Startup += (o, e) =>
            {
                CurrentTheme theme = SystemRegistrySetting.GetCurrentTheme();
                System.Drawing.Icon taskbarIcon
                = theme switch
                {
                    CurrentTheme.Dark => AudioSelector.Properties.Resources.appicon_white,
                    CurrentTheme.Light or CurrentTheme.Invalid => AudioSelector.Properties.Resources.appicon_black,
                    _ => AudioSelector.Properties.Resources.appicon_black,
                };

                taskbarControl = new()
                {
                    Text = "AudioSelector\nPress Ctrl+Alt+V to show selector window",
                    Icon = taskbarIcon,
                    Visible = true
                };
                taskbarControl.DoubleClick += OnTaskIconDoubleClick;

                contextMenu = new();
                taskbarControl.ContextMenuStrip
                    = contextMenu.ContextMenu;

                enumerationEvent = new();
                enumerationEvent.Start();
                enumerationEvent.Add += OnDeviceAdd;
                enumerationEvent.Remove += OnDeviceRemoved;

                viewModel = new AudioSelectorViewModel
                {
                    Devices = new ObservableCollection<MultiMediaDevice>(enumerationEvent.Devices)
                };

                Current.MainWindow = new MainWindow
                {
                    DataContext = viewModel
                };

                var service = new ServiceCollection();
                service.AddSingleton(viewModel);
                service.AddSingleton(MainWindow);
                container = service.BuildServiceProvider();


                hotKey = new(ID,
                            (ushort)(GlobalHotKey.MOD_CONTROL | GlobalHotKey.MOD_ALT),
                            0x56 /* V */);
                GlobalHotKey.HotKeyDown += OnKeyChange;
                hotKey.Start();
            };

            Exit += (o, e) =>
            {
                hotKey.Stop();
                enumerationEvent.Stop();
                enumerationEvent.Add -= OnDeviceAdd;
                enumerationEvent.Remove -= OnDeviceRemoved;
            };

        }

        private void OnTaskIconDoubleClick(object sender, EventArgs e)
        {
            ShowSelectWindow();
        }

        private void OnKeyChange(int param)
        {
            ShowSelectWindow();
        }

        /// <summary>
        /// Show audio device selector window
        /// </summary>
        private void ShowSelectWindow()
        {
            if(enumerationEvent.Devices.Count == 0)
            {
                Debug.WriteLine($"[App.ShowSelectWindow] No device listed");
                return;
            }

            try
            {
                taskbarControl.Visible = false;
                //Current.MainWindow
                //Current.MainWindow.Show();
                //if (Current.MainWindow.Activate())
                //{
                //    Debug.WriteLine($"[App.ShowSelectWindow] Activate successful");
                //}
                container.GetRequiredService<Window>().Show();
                if (container.GetRequiredService<Window>().Activate())
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
