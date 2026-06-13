using System.ComponentModel;

namespace AudioSelector.Setting
{
    public class GeneralViewModel : INotifyPropertyChanged
    {
        private bool _autoStart;

        private int _themeIndex;
        private int _languageIndex;
        private int _trayDoubleClickTargetIndex;

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

        public int LanguageIndex
        {
            get { return _languageIndex; }
            set
            {
                if (_languageIndex != value)
                {
                    _languageIndex = value;
                    OnPropertyChanged(nameof(LanguageIndex));
                }
            }
        }

        public bool AutoStart
        {
            get { return _autoStart; }
            set
            {
                if (_autoStart != value)
                {
                    _autoStart = value;
                    OnPropertyChanged(nameof(AutoStart));
                }
            }
        }

        public int TrayDoubleClickTargetIndex
        {
            get { return _trayDoubleClickTargetIndex; }
            set
            {
                if (_trayDoubleClickTargetIndex != value)
                {
                    _trayDoubleClickTargetIndex = value;
                    OnPropertyChanged(nameof(TrayDoubleClickTargetIndex));
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
            _themeIndex = (int)config.Property.Theme;
            _languageIndex = LanguageConverter.GetIndexFromSupportedLanguage(config.Property.Language);
            _autoStart = config.Property.Startup;
            _trayDoubleClickTargetIndex = config.Property.TrayDoubleClickTarget == TrayDoubleClickTarget.Microphone ? 1 : 0;
        }
    }
}
