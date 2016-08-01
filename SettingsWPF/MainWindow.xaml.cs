using MySettings;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SettingsWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            InitSettings();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Settings.ShowSettingsWindow(this);
            ColorPickerWindow sw = new ColorPickerWindow();
            sw.Show();
        }

        private void InitSettings()
        {
            //try
            //{
            //Settings.FileNameXml = @"newxml.xml";
            //Settings.SettingsWindowTitle = "Настройки программы";
            //Settings.LabelsWidth = 150.0;
            //Settings.LoadFromXml();
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message, "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            //    throw e;
            //}
        }

        public void Save()
        {
            Settings.SaveToXml();
        }
    }
}
