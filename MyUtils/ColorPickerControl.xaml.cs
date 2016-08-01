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

namespace MyUtils
{
    /// <summary>
    /// Логика взаимодействия для ColorPickerControl.xaml
    /// </summary>
    public partial class ColorPickerControl : UserControl
    {
        List<Color> Colors;
        Color SelectedColor;

        public static readonly DependencyProperty SelectedItemProperty;

        static ColorPickerControl()
        {
            SelectedItemProperty = DependencyProperty.RegisterAttached("SelectedItem",
              typeof(Color), typeof(ColorPickerControl),
              new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true });
        }

        public Color SelectedItem
        {
            get { return (Color)ColorMatrix.SelectedItem; }
            set { ColorMatrix.SelectedItem = value; }
        }

        public ColorPickerControl()
        {
            InitializeComponent();

            //Colors = new List<Color>()
            //{
            //    (Color)ColorConverter.ConvertFromString("#D32F2F"),
            //    (Color)ColorConverter.ConvertFromString("#7B1FA2"),
            //    (Color)ColorConverter.ConvertFromString("#1976D2"),
            //    (Color)ColorConverter.ConvertFromString("#00796B"),
            //    (Color)ColorConverter.ConvertFromString("#388E3C"),
            //    (Color)ColorConverter.ConvertFromString("#FBC02D"),
            //    (Color)ColorConverter.ConvertFromString("#F57C00"),
            //    (Color)ColorConverter.ConvertFromString("#5D4037"),
            //    (Color)ColorConverter.ConvertFromString("#000000"),
            //    (Color)ColorConverter.ConvertFromString("#616161"),
            //    (Color)ColorConverter.ConvertFromString("#FFFFFF")
            //};

            //ColorMatrix.ItemsSource = Colors;
            SelectedItem = (Color)ColorMatrix.Items[1];
            
        }
    }
}
