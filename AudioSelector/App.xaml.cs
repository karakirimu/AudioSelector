﻿using AudioSourceSelector.AudioDevice;
using HotKeyEvent;
using System;
using System.Diagnostics;
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

                taskbarControl = new();
                taskbarControl.Text = "AudioSourceSelector\nPress Ctrl+Alt+V to show selector window";
                taskbarControl.Icon = taskbarIcon;
                taskbarControl.Visible = true;
                taskbarControl.DoubleClick += OnTaskIconDoubleClick;

                contextMenu = new();
                taskbarControl.ContextMenuStrip
                    = contextMenu.ContextMenu;

                Current.MainWindow = new MainWindow();

                enumerationEvent = new();
                enumerationEvent.Start();

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
                Current.MainWindow.Show();
                if (Current.MainWindow.Activate())
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
    }
}
