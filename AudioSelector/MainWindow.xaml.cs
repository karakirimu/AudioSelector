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
            button.Click += OnButtonItemClick;
            return button;
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
