using AudioTools;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace AudioSelector.Setting
{
    /// <summary>
    /// DeviceHotKey.xaml の相互作用ロジック
    /// </summary>
    public partial class DeviceHotKey : UserControl
    {
        private readonly AudioDeviceKind deviceKind;
        private DeviceHotKeyViewModel viewModel;
        private IAppConfig appConfig;

        public DeviceHotKey(IAppConfig config, AudioDeviceKind kind)
        {
            InitializeComponent();
            deviceKind = kind;

            Loaded += (o, e) =>
            {
                viewModel = new DeviceHotKeyViewModel(config, deviceKind);
                viewModel.PropertyChanged += DeviceHotKeyPropertyChanged;
                appConfig = config;
                DataContext = viewModel;
            };

        }

        private void DeviceHotKeyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(viewModel.HotKeyEnabled))
            {
                SetHotKeyEnabled(viewModel.HotKeyEnabled);
                return;
            }

            if(e.PropertyName == nameof(viewModel.ModifierCtrl)
                || e.PropertyName == nameof(viewModel.ModifierShift)
                || e.PropertyName == nameof(viewModel.ModifierAlt)
                || e.PropertyName == nameof(viewModel.ModifierWin)
                || e.PropertyName == nameof(viewModel.VKey))
            {
                SetHotKey(CreateHotKey());
                return;
            }
        }

        private void LineEdit_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            lineEdit.Text = e.Key.ToString();
            lineEdit.CaretIndex = lineEdit.Text.Length;

            HotKey result = CreateHotKey(e.Key.ToString());
            SetHotKey(result);
            e.Handled = true;
        }

        private HotKey CreateHotKey(string virtualKey = null)
        {
            return new HotKey()
            {
                Ctrl = viewModel.ModifierCtrl,
                Shift = viewModel.ModifierShift,
                Alt = viewModel.ModifierAlt,
                Win = viewModel.ModifierWin,
                VirtualKey = virtualKey ?? viewModel.VKey
            };
        }

        private void SetHotKeyEnabled(bool enabled)
        {
            if (deviceKind == AudioDeviceKind.Microphone)
            {
                appConfig?.SetMicrophoneHotKeyEnabled(enabled);
                return;
            }

            appConfig?.SetSpeakerHotKeyEnabled(enabled);
        }

        private void SetHotKey(HotKey hotkey)
        {
            if (deviceKind == AudioDeviceKind.Microphone)
            {
                appConfig?.SetMicrophoneHotkey(hotkey);
                return;
            }

            appConfig?.SetSpeakerHotkey(hotkey);
        }
    }
}
