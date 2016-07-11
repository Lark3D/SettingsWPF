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
            Settings.ShowSettingsWindow(this);
        }

        private void InitSettings()
        {
            try
            {

                Settings.SettingsFileName = "set.ini";
                Settings.SettingsWindowTitle = "Настройки программы";
                Settings.LabelsWidth = 150.0;

                string cat1 = "Фон схемы";
                string cat2 = "Категория 2";

                Settings.InitMarkupSetting(cat1, MarkupType.SubCategory, "Раздел 1 (глупый)");
                Settings.InitStringSetting(cat1, "schemebg", "Вид фона", "Сетка", false, null, "Нет", "Сетка", "Точки");
                Settings.InitBoolSetting(cat1, "isallok", "Все хорошо?", false);
                Settings.InitMarkupSetting(cat1, MarkupType.Information, "Единственно верным ответом может быть FALSE");
                Settings.InitMarkupSetting(cat1, MarkupType.SubCategory, "Раздел 2 (очень глупый)");
                Settings.InitStringSetting(cat1, "rndstr", "Случайная строка:", "123", false, "Введите что-дь бла бла бла! И еще очень длинная строка... Даже еще длиннее.\nНу и перенос строки до кучи.");

                Settings.InitMarkupSetting(cat2, MarkupType.SubCategory, "Раздел 3 (Умный)");
                Settings.InitIntegerSetting(cat2, "batmanstr", "Сила бэтмана:", 27, 100, 0);
                Settings.InitIntegerSetting(cat2, "supermanstr", "Сила супермэна:", 33, null, null, 33, 7);
                Settings.InitMarkupSetting(cat2, MarkupType.SubCategory, "Раздел 4 (Очень умный!)");
                Settings.InitDoubleSetting(cat2, "batmanweight", "Вес бэтмана:", 70.0, 1000.0, 0.0);
                Settings.InitDoubleSetting(cat2, "supermanweight", "Вес супермэна:", 119.4, 1000.0, 0.0, 119.4, 200.0);
                Settings.InitPathSetting(cat2, "dbpath", "Пусть к базе данных:", @".\Tasks", true, true);
                Settings.InitFileSetting(cat2, "dbfilename", "Файл базы данных:", @"default.ac", false, false, "Файлы GuTestAC|*.ac|Файлы GuTestDC|*.dc|Все файлы|*.*");
                Settings.InitFileSetting(cat2, "dbfilename2", "Файл базы данных 2:", @"set.ini", false, false, "Файлы настроек|*.ini|Все файлы|*.*");

                Settings.Load();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                throw e;
            }
        }
    }
}
