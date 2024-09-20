using System.ComponentModel;

namespace AudioSelector.Setting
{
    public class GeneralViewModel : INotifyPropertyChanged
    {
        private bool _modifierCtrl;
        private bool _modifierShift;
        private bool _modifierAlt;
        private bool _modifierWin;

        private string _vKey;

        private int _themeIndex;

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

        public int ThemeIndex
        {
            get { return _themeIndex; }
            set
            {
                if (_themeIndex != value)
                {
                    _themeIndex = value;
                    OnPropertyChanged(nameof(ThemeIndex));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public GeneralViewModel(IAppConfig config)
        {
            _modifierShift = config.Property.Hotkey.Shift;
            _modifierCtrl = config.Property.Hotkey.Ctrl;
            _modifierAlt = config.Property.Hotkey.Alt;
            _modifierWin = config.Property.Hotkey.Win;
            _vKey = config.Property.Hotkey.VirtualKey;
            _themeIndex = (int)config.Property.Theme;
        }
    }
}
