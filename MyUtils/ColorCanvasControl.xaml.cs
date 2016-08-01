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
    public partial class ColorCanvasControl : UserControl
    {
        private Point _currentColorPosition;

        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.RegisterAttached("SelectedColor", 
                                                  typeof(Color), typeof(ColorCanvasControl), 
                                                  new FrameworkPropertyMetadata(Color.FromArgb(255, 255, 255, 255), 
                                                  FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedColorChanged));
        public Color SelectedColor
        {
            get
            {
                return (Color)GetValue(SelectedColorProperty);
            }
            set
            {
                SetValue(SelectedColorProperty, value);
            }
        }

        private Color _selectedHue;
        public Color SelectedHue
        {
            get
            {
                return _selectedHue;
            }
            set
            {
                _selectedHue = value;
            }
        }

        private static void OnSelectedColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColorCanvasControl colorCanvas = o as ColorCanvasControl;
            if (colorCanvas != null)
                colorCanvas.OnSelectedColorChanged((Color)e.OldValue, (Color)e.NewValue);
        }

        protected virtual void OnSelectedColorChanged(Color oldValue, Color newValue)
        {
            RoutedPropertyChangedEventArgs<Color> args = new RoutedPropertyChangedEventArgs<Color>(oldValue, newValue);
            args.RoutedEvent = SelectedColorChangedEvent;
            RaiseEvent(args);
        }
        //public static readonly DependencyProperty SelectedItemProperty;
        public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent("SelectedColorChanged", RoutingStrategy.Bubble, 
                                                                            typeof(RoutedPropertyChangedEventHandler<Color>), typeof(ColorCanvasControl));


        static ColorCanvasControl()
        {
            //SelectedItemProperty = DependencyProperty.RegisterAttached("SelectedItem",
            //  typeof(Color), typeof(ColorPickerControl),
            //  new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true });
        }


        public ColorCanvasControl()
        {
            InitializeComponent();

            ColorSelectorTransform = new TranslateTransform();

            if (ColorSelector != null)
            {
                ColorSelector.RenderTransform = ColorSelectorTransform;
            }

            SpectreSlider.Background = SpectreBrush();
        }

        #region RGB

        #region A

        public static readonly DependencyProperty AProperty = DependencyProperty.Register("A", typeof(byte), typeof(ColorCanvasControl), new UIPropertyMetadata((byte)255, OnAChanged));
        public byte A
        {
            get
            {
                return (byte)GetValue(AProperty);
            }
            set
            {
                SetValue(AProperty, value);
            }
        }

        private static void OnAChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColorCanvasControl ColorCanvasControl = o as ColorCanvasControl;
            if (ColorCanvasControl != null)
                ColorCanvasControl.OnAChanged((byte)e.OldValue, (byte)e.NewValue);
        }

        protected virtual void OnAChanged(byte oldValue, byte newValue)
        {
                UpdateSelectedColor();
        }

        #endregion //A

        #region R

        public static readonly DependencyProperty RProperty = DependencyProperty.Register("R", typeof(byte), typeof(ColorCanvasControl), new UIPropertyMetadata((byte)0, OnRChanged));
        public byte R
        {
            get
            {
                return (byte)GetValue(RProperty);
            }
            set
            {
                SetValue(RProperty, value);
            }
        }

        private static void OnRChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColorCanvasControl ColorCanvasControl = o as ColorCanvasControl;
            if (ColorCanvasControl != null)
                ColorCanvasControl.OnRChanged((byte)e.OldValue, (byte)e.NewValue);
        }

        protected virtual void OnRChanged(byte oldValue, byte newValue)
        {
                UpdateSelectedColor();
        }

        #endregion //R

        #region G

        public static readonly DependencyProperty GProperty = DependencyProperty.Register("G", typeof(byte), typeof(ColorCanvasControl), new UIPropertyMetadata((byte)0, OnGChanged));
        public byte G
        {
            get
            {
                return (byte)GetValue(GProperty);
            }
            set
            {
                SetValue(GProperty, value);
            }
        }

        private static void OnGChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColorCanvasControl ColorCanvasControl = o as ColorCanvasControl;
            if (ColorCanvasControl != null)
                ColorCanvasControl.OnGChanged((byte)e.OldValue, (byte)e.NewValue);
        }

        protected virtual void OnGChanged(byte oldValue, byte newValue)
        {
                UpdateSelectedColor();
        }

        #endregion //G

        #region B

        public static readonly DependencyProperty BProperty = DependencyProperty.Register("B", typeof(byte), typeof(ColorCanvasControl), new UIPropertyMetadata((byte)0, OnBChanged));
        public byte B
        {
            get
            {
                return (byte)GetValue(BProperty);
            }
            set
            {
                SetValue(BProperty, value);
            }
        }

        public TranslateTransform ColorSelectorTransform { get; private set; }

        private static void OnBChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColorCanvasControl ColorCanvasControl = o as ColorCanvasControl;
            if (ColorCanvasControl != null)
                ColorCanvasControl.OnBChanged((byte)e.OldValue, (byte)e.NewValue);
        }

        protected virtual void OnBChanged(byte oldValue, byte newValue)
        {
                UpdateSelectedColor();
        }

        #endregion //B

        #endregion //RGB

        private void UpdateSelectedColor()
        {
            SelectedColor = Color.FromArgb(A, R, G, B);
        }

        private void UpdateSelectedColor(Color color)
        {
            Color.FromArgb(color.A, color.R, color.G, color.B);
        }


        private void UpdateColorShadeSelectorPositionAndCalculateColor(Point p, bool calculateColor)
        {
            if (p.Y < 0)
                p.Y = 0;

            if (p.X < 0)
                p.X = 0;

            if (p.X > ColorCanvas.ActualWidth)
                p.X = ColorCanvas.ActualWidth;

            if (p.Y > ColorCanvas.ActualHeight)
                p.Y = ColorCanvas.ActualHeight;

            ColorSelectorTransform.X = p.X - (ColorSelector.Width / 2);
            ColorSelectorTransform.Y = p.Y - (ColorSelector.Height / 2);

            p.X = p.X / ColorCanvas.ActualWidth;
            p.Y = p.Y / ColorCanvas.ActualHeight;

            _currentColorPosition = p;

            if (calculateColor)
                CalculateColor(p);
        }

        private void CalculateColor(Point p)
        {
            HsvColor hsv = new HsvColor(360 - SpectreSlider.Value, 1, 1)
            {
                S = p.X,
                V = 1 - p.Y
            };
            var currentColor = ColorUtilities.ConvertHsvToRgb(hsv.H, hsv.S, hsv.V);
            currentColor.A = A;
            SelectedColor = currentColor;
        }

        void ColorCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(ColorCanvas);
            UpdateColorShadeSelectorPositionAndCalculateColor(p, true);
        }

        void ColorCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ColorCanvas.ReleaseMouseCapture();
        }

        void ColorCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = e.GetPosition(ColorCanvas);
                UpdateColorShadeSelectorPositionAndCalculateColor(p, true);
                Mouse.Synchronize();
            }
        }

        void ColorCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_currentColorPosition != null)
            {
                Point _newPoint = new Point
                {
                    X = ((Point)_currentColorPosition).X * e.NewSize.Width,
                    Y = ((Point)_currentColorPosition).Y * e.NewSize.Height
                };

                UpdateColorShadeSelectorPositionAndCalculateColor(_newPoint, false);
            }
        }


        void SpectreCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(ColorCanvas);
            UpdateColorShadeSelectorPositionAndCalculateColor(p, true);
        }

        void SpectreCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ColorCanvas.ReleaseMouseCapture();
        }

        LinearGradientBrush SpectreBrush()
        {
            LinearGradientBrush newBrush = new LinearGradientBrush();
            newBrush.StartPoint = new Point(0.5, 0);
            newBrush.EndPoint = new Point(0.5, 1);
            
            double n = 40.0;
            for (int i = 0; i < n; i++)
            {
                Color newColor = ColorUtilities.ConvertHsvToRgb(360 * i / n, 1, 1);
                GradientStop newGradientStop = new GradientStop(newColor, i / n);
                newBrush.GradientStops.Add(newGradientStop);
            }

            return newBrush;
        }

        private void SpectreSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SelectedHue = ColorUtilities.ConvertHsvToRgb(e.NewValue, 1, 1);
            ColorShadingRectangle.Fill = new SolidColorBrush(SelectedHue);
        }
    }
}
