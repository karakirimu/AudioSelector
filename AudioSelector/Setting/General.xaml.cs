using System.ComponentModel;
using System.Windows.Controls;

namespace AudioSelector.Setting
{
    /// <summary>
    /// General.xaml の相互作用ロジック
    /// </summary>
    public partial class General : UserControl
    {
        private GeneralViewModel viewModel;
        private IAppConfig appConfig;

        public General(IAppConfig config)
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                viewModel = new GeneralViewModel(config);
                viewModel.PropertyChanged += GeneralSettingPropertyChanged;
                appConfig = config;
                DataContext = viewModel;
            };

        }

        private void GeneralSettingPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(viewModel.AutoStart))
            {
                appConfig?.SetStartup(viewModel.AutoStart);
                return;
            }
        }

        private void ThemeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (themeComboBox.SelectedItem != null && themeComboBox.SelectedItem is ComboBoxItem comboBoxItem)
            {
                var theme = (SystemTheme)(int.Parse(comboBoxItem.Tag.ToString()));
                appConfig?.SetTheme(theme);
            }
        }
        private void LanguageSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (languageComboBox.SelectedItem != null && languageComboBox.SelectedItem is ComboBoxItem comboBoxItem)
            {
                var lang = (string)comboBoxItem.Content;
                string cultureName = LanguageConverter.GetSupportedLanguage(lang);
                appConfig?.SetLanguage(cultureName);
            }
        }

        private void TrayDoubleClickTargetSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (trayDoubleClickTargetComboBox.SelectedItem != null && trayDoubleClickTargetComboBox.SelectedItem is ComboBoxItem comboBoxItem)
            {
                var target = (TrayDoubleClickTarget)(int.Parse(comboBoxItem.Tag.ToString()));
                appConfig?.SetTrayDoubleClickTarget(target);
            }
        }
    }
}
