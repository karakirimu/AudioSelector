using System.Windows;
using System.Windows.Controls;

namespace AudioSelector.Setting
{
    /// <summary>
    /// Settings.xaml の相互作用ロジック
    /// </summary>
    public partial class Settings : Window
    {
        private IAppConfig AppConfig;
        public Settings(IAppConfig config)
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                AppConfig = config;
                settingsListBox.SelectedIndex = 0;
            };

            Closing += (o, e) =>
            {
                e.Cancel = true;
                this.Visibility = Visibility.Collapsed;
            };

        }

        private void SettingsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (settingsListBox.SelectedItem != null)
            {
                string selectedSetting = (settingsListBox.SelectedItem as ListBoxItem).Name;
                switch (selectedSetting)
                {
                    case "General":
                        settingsContentControl.Content = new General(AppConfig);
                        break;
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
