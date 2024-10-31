using AudioTools;
using NativeCoreAudio;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AudioSelector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Selector UI element.
        /// </summary>
        private readonly Dictionary<string, RadioButton> deviceCollection;

        /// <summary>
        /// This value sets audio device select menu column count.
        /// </summary>
        private static readonly int columnSize = 4;

        /// <summary>
        /// MainWindow constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            deviceCollection = [];

            Loaded += (o, e) =>
            {
                Debug.WriteLine("[MainWindow.Loaded]");
                AudioSelectorViewModel model = DataContext as AudioSelectorViewModel;
                model.Devices.CollectionChanged += DevicesCollectionChanged;
                model.VolumeChangeEvent.Update += OnDeviceVolumeUpdate;
            };

            Activated += (o, e) =>
            {
                Debug.WriteLine("[MainWindow.Activated]");
            };

            Deactivated += (o, e) =>
            {
                Debug.WriteLine("[MainWindow.Deactivated]");
                Hide();
            };

            SizeChanged += (o, e) =>
            {
                double xmove = (e.NewSize.Width - e.PreviousSize.Width) / 2;
                double ymove = (e.NewSize.Height - e.PreviousSize.Height) / 2;

                Left -= xmove;
                Top -= ymove;
            };

            MouseEnter += (o, e) =>
            {
                Keyboard.ClearFocus();
            };

            IsVisibleChanged += (o, e) =>
            {
                Debug.WriteLine("[MainWindow.IsVisibleChanged]");
                if ((bool)e.NewValue == true)
                {
                    AudioSelectorViewModel model = DataContext as AudioSelectorViewModel;
                    UpdateDeviceList(model.Devices);
                }
            };

        }

        private void DevicesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Debug.WriteLine("[MainWindow.DevicesCollectionChanged]");
            ObservableCollection<MultiMediaDevice> device = sender as ObservableCollection<MultiMediaDevice>;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Debug.WriteLine("[MainWindow.DevicesCollectionChanged] NotifyCollectionChangedAction.Add");
                    foreach (MultiMediaDevice m in e.NewItems)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            RadioButton selectbutton = CreateButtonItem(m.Id, m.DeviceName);
                            deviceCollection.Add(m.Id, selectbutton);
                            AudioList.Columns = GetAudioListColumnCount(deviceCollection.Count);

                            UpdateDeviceList(device);
                        });
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    Debug.WriteLine("[MainWindow.DevicesCollectionChanged] NotifyCollectionChangedAction.Remove");
                    foreach (MultiMediaDevice m in e.OldItems)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            _ = deviceCollection.Remove(m.Id);
                            UpdateDeviceList(device);
                        });
                    }
                    break;
                default:
                    break;
            }
        }

        private void AudioListKeyDown(object sender, KeyEventArgs e)
        {
            if (deviceCollection.Count < 1)
            {
                return;
            }

            int index = GetKeyboardFocusedIndex(AudioList.Children);

            switch (e.Key)
            {
                case Key.Tab:
                    index = GetMovedIndex(index, AudioList.Children.Count);
                    _ = AudioList.Children[index].Focus();
                    break;

                case Key.Enter:
                case Key.Separator:
                    RadioButton button = AudioList.Children[index] as RadioButton;
                    SetDefaultEndpoint(button.Tag as string);
                    Hide();
                    break;

                default:
                    break;
            }
        }

        private void UpdateDeviceList(IReadOnlyCollection<MultiMediaDevice> devices)
        {
            deviceCollection.Clear();
            AudioList.Children.Clear();
            AudioList.Columns = GetAudioListColumnCount(devices.Count);

            string defaultId
                = Enumeration.GetDefaultDeviceEndpointId(ComInterfaces.ERole.eConsole);

            foreach (MultiMediaDevice device in devices)
            {
                RadioButton selectbutton = CreateButtonItem(device.Id, device.DeviceName);

                if (device.Id == defaultId)
                {
                    selectbutton.IsChecked = true;
                }

                deviceCollection.Add(device.Id, selectbutton);
                _ = AudioList.Children.Add(selectbutton);
            }
        }

        private static int GetAudioListColumnCount(int count)
        {
            return count < 1 ? 0 : count < columnSize ? count : columnSize;
        }

        /// <summary>
        /// Initialize Audio device list item button
        /// </summary>
        /// <param name="id">device id</param>
        /// <param name="devicename">device name</param>
        /// <returns>Initialized button</returns>
        private RadioButton CreateButtonItem(string id, string devicename)
        {
            RadioButton button = new()
            {
                Content = devicename,
                Tag = id,
                IsChecked = false,
                GroupName = "AudioDevices",
                Style = (Style)FindResource("SelectorRadioButtonStyle"),
            };

            button.ApplyTemplate();

            TextBlock volumeIcon = (TextBlock)button.Template.FindName("VolumeIcon", button);
            volumeIcon.FontFamily = new FontFamily("Segoe Fluent Icons");
            volumeIcon.Text = VolumeIconSelect(Enumeration.GetMute(id), Enumeration.GetMasterVolume(id));

            button.Click += OnButtonItemClick;
            return button;
        }

        /// <summary>
        /// Update Audio device list item volume icon
        /// </summary>
        /// <param name="args">new volume information</param>
        /// <returns>HRESULT</returns>
        private uint OnDeviceVolumeUpdate(AudioVolumeNotificationData args)
        {
            if (deviceCollection.TryGetValue(args.deviceId, out RadioButton value))
            {
                TextBlock volumeIcon = (TextBlock)value.Template.FindName("VolumeIcon", value);

                if (volumeIcon != null)
                {
                    volumeIcon.Dispatcher.Invoke(() =>
                    {
                        volumeIcon.FontFamily = new FontFamily("Segoe Fluent Icons");
                        volumeIcon.Text = VolumeIconSelect(args.muted, args.masterVolume);
                        volumeIcon.InvalidateVisual();
                    });
                }
            }
            
            return 0;
        }

        private static string VolumeIconSelect(bool muted, float masterVolume)
        {
            int vol = (int)(masterVolume * 1000000);

            if (muted)
            {
                return "\uE74F";
            }

            if(vol > 666666)
            {
                return "\uE995";
            }

            if (vol > 333333)
            {
                return "\uE994";
            }

            if (vol > 1)
            {
                return "\uE993";
            }

            return "\uE992";
        }

        private void OnButtonItemClick(object sender, RoutedEventArgs e)
        {
            RadioButton clicked = sender as RadioButton;
            Debug.WriteLine(clicked.Tag);
            SetDefaultEndpoint(clicked.Tag as string);
            Hide();
        }

        private static void SetDefaultEndpoint(string id)
        {
            using SafeIPolicyConfig config = new();

            try
            {
                config.SetDefaultEndpoint(id, ComInterfaces.ERole.eConsole);
                config.SetDefaultEndpoint(id, ComInterfaces.ERole.eMultimedia);
                config.SetDefaultEndpoint(id, ComInterfaces.ERole.eCommunications);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private static int GetMovedIndex(int index, int maxcount)
        {
            index++;
            return maxcount == index ? 0 : index;
        }

        private static int GetKeyboardFocusedIndex(UIElementCollection buttons)
        {
            int resultindex = -1;
            if (buttons.Count < 1)
            {
                return resultindex;
            }

            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i].IsKeyboardFocused)
                {
                    return i;
                }
            }

            return resultindex;
        }
    }
}
