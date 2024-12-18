using AudioSelector.Setting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;

namespace AudioSelector.Properties
{
    internal class DynamicResource
    {
        private const string LIGHT_THEME = @".\Properties\Light.xaml";
        private const string DARK_THEME = @".\Properties\Dark.xaml";
        private readonly ReadOnlyDictionary<string, string> LANGUAGE_RESOURCE_PATH
            = new(
                new Dictionary<string, string>(){
                    { "en-US", @".\Properties\Resources.xaml" },
                    { "ja-JP", @".\Properties\Resources.ja-JP.xaml" }
                }
            );

        private SystemTheme Theme = SystemTheme.System;
        private string LanguageCode = "System";


        public void UpdateTheme(SystemTheme theme)
        {
            Theme = theme;
            UpdateMergedDictionaries();
        }

        public void UpdateLanguage(string languageCode)
        {
            LanguageCode = languageCode;
            UpdateMergedDictionaries();
        }

        private void UpdateMergedDictionaries()
        {
            Application.Current.Resources.MergedDictionaries.Clear();

            // Add Theme Resource
            switch (Theme)
            {
                case SystemTheme.Light:
                    Application.Current.Resources.MergedDictionaries.Add(
                        new ResourceDictionary() { Source = new Uri(LIGHT_THEME, UriKind.Relative) });
                    break;
                case SystemTheme.Dark:
                    Application.Current.Resources.MergedDictionaries.Add(
                        new ResourceDictionary() { Source = new Uri(DARK_THEME, UriKind.Relative) });
                    break;
                case SystemTheme.System:
                    SystemTheme theme = SystemRegistry.GetCurrentTheme();
                    if (theme == SystemTheme.Dark)
                    {
                        Application.Current.Resources.MergedDictionaries.Add(
                            new ResourceDictionary() { Source = new Uri(DARK_THEME, UriKind.Relative) });
                    }
                    else
                    {
                        Application.Current.Resources.MergedDictionaries.Add(
                            new ResourceDictionary() { Source = new Uri(LIGHT_THEME, UriKind.Relative) });
                    }
                    break;
            }

            // Add Language Resource
            if (LANGUAGE_RESOURCE_PATH.TryGetValue(LanguageCode, out string value))
            {
                Application.Current.Resources.MergedDictionaries.Add(
                    new ResourceDictionary() { Source = new Uri(value, UriKind.Relative) });
                return;
            }

            Application.Current.Resources.MergedDictionaries.Add(
                new ResourceDictionary() { Source = new Uri(LANGUAGE_RESOURCE_PATH["en-US"], UriKind.Relative) });

        }
    }
}
