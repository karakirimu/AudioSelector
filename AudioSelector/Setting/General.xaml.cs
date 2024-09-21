﻿using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

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
            HotKey result = new()
            {
                Ctrl = viewModel.ModifierCtrl,
                Shift = viewModel.ModifierShift,
                Alt = viewModel.ModifierAlt,
                Win = viewModel.ModifierWin,
                VirtualKey = viewModel.VKey
            };

            appConfig?.SetHotkey(result);
        }

        private void LineEdit_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            lineEdit.Text = e.Key.ToString();
            lineEdit.CaretIndex = lineEdit.Text.Length;

            HotKey result = new()
            {
                Ctrl = viewModel.ModifierCtrl,
                Shift = viewModel.ModifierShift,
                Alt = viewModel.ModifierAlt,
                Win = viewModel.ModifierWin,
                VirtualKey = e.Key.ToString()
            };

            appConfig?.SetHotkey(result);
            e.Handled = true;
        }

        private void ThemeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (themeComboBox.SelectedItem != null && themeComboBox.SelectedItem is ComboBoxItem comboBoxItem)
            {
                var theme = Enum.Parse<SystemTheme>((string)comboBoxItem.Content);
                appConfig?.SetTheme(theme);
            }
        }
    }
}