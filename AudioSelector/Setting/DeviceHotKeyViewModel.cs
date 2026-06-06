using AudioTools;
using System.ComponentModel;

namespace AudioSelector.Setting
{
    public class DeviceHotKeyViewModel : INotifyPropertyChanged
    {
        private bool _hotKeyEnabled;
        private bool _modifierCtrl;
        private bool _modifierShift;
        private bool _modifierAlt;
        private bool _modifierWin;
        private string _vKey;

        public bool HotKeyEnabled
        {
            get { return _hotKeyEnabled; }
            set
            {
                if (_hotKeyEnabled != value)
                {
                    _hotKeyEnabled = value;
                    OnPropertyChanged(nameof(HotKeyEnabled));
                }
            }
        }

        public bool ModifierCtrl
        {
            get { return _modifierCtrl; }
            set
            {
                if (_modifierCtrl != value)
                {
                    _modifierCtrl = value;
                    OnPropertyChanged(nameof(ModifierCtrl));
                }
            }
        }

        public bool ModifierShift
        {
            get { return _modifierShift; }
            set
            {
                if (_modifierShift != value)
                {
                    _modifierShift = value;
                    OnPropertyChanged(nameof(ModifierShift));
                }
            }
        }

        public bool ModifierAlt
        {
            get { return _modifierAlt; }
            set
            {
                if (_modifierAlt != value)
                {
                    _modifierAlt = value;
                    OnPropertyChanged(nameof(ModifierAlt));
                }
            }
        }

        public bool ModifierWin
        {
            get { return _modifierWin; }
            set
            {
                if (_modifierWin != value)
                {
                    _modifierWin = value;
                    OnPropertyChanged(nameof(ModifierWin));
                }
            }
        }

        public string VKey
        {
            get { return _vKey; }
            set
            {
                if (_vKey != value)
                {
                    _vKey = value;
                    OnPropertyChanged(nameof(VKey));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DeviceHotKeyViewModel(IAppConfig config, AudioDeviceKind kind)
        {
            HotKey hotkey = kind == AudioDeviceKind.Microphone
                ? config.Property.MicrophoneHotkey
                : config.Property.SpeakerHotkey;

            _hotKeyEnabled = kind == AudioDeviceKind.Microphone
                ? config.Property.MicrophoneHotkeyEnabled
                : config.Property.SpeakerHotkeyEnabled;
            _modifierShift = hotkey.Shift;
            _modifierCtrl = hotkey.Ctrl;
            _modifierAlt = hotkey.Alt;
            _modifierWin = hotkey.Win;
            _vKey = hotkey.VirtualKey;
        }
    }
}
