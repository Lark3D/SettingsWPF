using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MySettings
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    internal partial class SettingsWindow : Window
    {
        internal SettingsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lbCategories.ItemsSource = Settings.SettingsCategories;

            lbCategories.SelectedIndex = 0;

        }

        private void lbCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadMainPanel();
        }

        public void LoadMainPanel()
        {
            //if (swrap == null) return;
            spMain.Children.Clear();

            SettingsCategory sc = (SettingsCategory)lbCategories.SelectedItem;

            if (sc == null) return;

            foreach (Setting s in sc.Settings)
            {
                spMain.Children.Add(s.GenerateContent());
            }

        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            Settings.SaveToXml();
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Load();
        }

    }

    internal class BooleanOrConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            foreach (object value in values)
            {
                if ((bool)value == true)
                {
                    return true;
                }
            }
            return false;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }


}
