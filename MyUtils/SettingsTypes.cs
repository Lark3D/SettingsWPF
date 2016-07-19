using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Data;
using System.ComponentModel;

namespace MySettings
{

    internal class SettingsCategory
    {
        public string Description { get; set; }
        public List<Setting> Settings;

        public SettingsCategory()
        {
            Settings = new List<Setting>();
        }

        public SettingsCategory(string description)
        {
            Description = description;
            Settings = new List<Setting>();
        }
    }

    internal abstract class Setting
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public abstract bool Validate(object value, out string msg);
        public abstract object GetValue();
        public abstract void SetValue(object value);
        public abstract void SetDefault();
        public abstract Border GenerateContent();

        protected void tb_MoveFocusOnEnter(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            TextBox t = sender as TextBox;
            
            t.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        protected Label MakeLabel(string text)
        {
            Label l = new Label();
            l.Content = text;
            l.MinWidth = Settings.LabelsWidth;
            l.VerticalAlignment = VerticalAlignment.Center;
            return l;
        }

        /*
        protected void tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            BindingOperations.GetBindingExpression(tb, TextBox.TextProperty).UpdateSource();

        }
        */
    }

    internal class BoolSetting : Setting
    {
        private bool _value;

        public bool Value
        {
            get { return _value; }
            set { SetValue(value); }
        }
        public bool DefaultValue;

        public bool n;

        public override bool Validate(object value, out string msg)
        {
            msg = "-";
            try
            {
                n = Convert.ToBoolean(value);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public override object GetValue()
        {
            return _value;
        }

        public override void SetValue(object value)
        {
            string msg;
            if (Validate(value, out msg)) _value = n;
        }

        public override void SetDefault()
        {
            string msg;
            if (Validate(DefaultValue, out msg)) _value = DefaultValue;
            else throw new BadDefaultValueException();
        }

        public override Border GenerateContent()
        {
            Border border = new Border();

            CheckBox cb = new CheckBox();
            cb.IsChecked = _value;
            cb.Content = Description;
            cb.Margin = new Thickness(5.0, 0.0, 0.0, 0.0);

            cb.DataContext = this;
            Binding bind = new Binding();
            bind.Path = new PropertyPath(typeof(BoolSetting).GetProperties()[0].Name);
            SettingValidationRule svr = new SettingValidationRule(this);
            bind.ValidationRules.Add(svr);
            bind.NotifyOnValidationError = true;
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            cb.SetBinding(CheckBox.IsCheckedProperty, bind);

            border.Margin = Settings.SettingsWindowElementsMargin;
            border.Child = cb;
            return border;
        }
    }

    internal class IntegerSetting : Setting
    {
        private int _value;

        public string BindingValue
        {
            get { return _value.ToString(); }
            set { SetValue(value); }
        }

        public int DefaultValue;
        public List<int> PossibleValues;
        public int? MaxPossibleValue;
        public int? MinPossibleValue;

        public int n;

        public IntegerSetting()
        {
            PossibleValues = new List<int>();
            MaxPossibleValue = null;
            MinPossibleValue = null;
        }

        public override bool Validate(object value, out string msg)
        {
            msg = "Не удалось распознать число.";
            if (value is string)
            {
                if (!int.TryParse((string)value, out n)) return false;
            }
            else
            {
                try
                {
                    n = Convert.ToInt32(value);
                }
                catch
                {
                    return false;
                }
            }

            msg = "Возможные варианты:";
            foreach (int i in PossibleValues)
            {
                msg += "  " + i.ToString();
            }
            if (PossibleValues.Count > 0)
            {
                if (!PossibleValues.Contains(n)) return false;
            }

            msg = "";
            if (MinPossibleValue != null) msg += " Min: " + MinPossibleValue.ToString();
            if (MaxPossibleValue != null) msg += " Max: " + MaxPossibleValue.ToString();

            if (MaxPossibleValue.HasValue && n > MaxPossibleValue) return false;
            if (MinPossibleValue.HasValue && n < MinPossibleValue) return false;
            msg = "";
            return true;
        }

        public override object GetValue()
        {
            return _value;
        }

        public override void SetValue(object value)
        {
            string msg;
            if (Validate(value, out msg)) _value = n;
        }

        public override void SetDefault()
        {
            string msg;
            if (Validate(DefaultValue, out msg)) _value = DefaultValue;
            else throw new BadDefaultValueException();
        }

        public override Border GenerateContent()
        {
            Border border = new Border();

            TextBox tb = new TextBox();
            tb.VerticalAlignment = VerticalAlignment.Center;
            tb.Width = 50;
            tb.DataContext = this;
            tb.KeyUp += new KeyEventHandler(tb_MoveFocusOnEnter);

            Binding bind = new Binding();
            bind.Path = new PropertyPath(typeof(IntegerSetting).GetProperties()[0].Name);
            SettingValidationRule svr = new SettingValidationRule(this);
            bind.ValidationRules.Add(svr);
            bind.NotifyOnValidationError = true;
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            tb.SetBinding(TextBox.TextProperty, bind);

            Label l = MakeLabel(Description);

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.Children.Add(l);
            sp.Children.Add(tb);

            border.Margin = Settings.SettingsWindowElementsMargin;
            border.Child = sp;
            return border;

        }


    }

    internal class DoubleSetting : Setting
    {
        private double dvalue;
        private string svalue;

        public string Value
        {
            get 
            {
                // все эти пляски - чтобы валидация по TextChanged выполненная через UpdateSource() не удаляла только что введенный
                // десятичный разделитель, иначе при вводе "3." строка преобразуется в 3 и обратно в строковую "3"
                string msg;
                if (Validate(svalue, out msg) && n == dvalue) return svalue;
                else return dvalue.ToString();
            }
            set
            {
                svalue = value;
                SetValue(value);
            }
        }
        public double DefaultValue;
        public List<double> PossibleValues;
        public double? MaxPossibleValue;
        public double? MinPossibleValue;

        public double n; // это по сути, результат конвертации строки в число при вызове Validate

        public DoubleSetting()
        {
            PossibleValues = new List<double>();
            MaxPossibleValue = null;
            MinPossibleValue = null;
        }

        public override bool Validate(object value, out string msg)
        {
            msg = "Не удалось распознать число.";
            if (value is string)
            {
                if (!double.TryParse((string)value, out n))
                    if (!double.TryParse((string)value, NumberStyles.Float, CultureInfo.InvariantCulture, out n))
                        return false;
            }
            else
            {
                try
                {
                    n = Convert.ToDouble(value);
                }
                catch
                {
                    return false;
                }
            }

            msg = "Возможные варианты:";
            foreach (double i in PossibleValues)
            {
                msg += " " + i.ToString();
            }
            if (PossibleValues.Count > 0)
            {
                if (!PossibleValues.Contains(n)) return false;
            }

            msg = "";
            if (MinPossibleValue != null) msg += " Min: " + MinPossibleValue.ToString();
            if (MaxPossibleValue != null) msg += " Max: " + MaxPossibleValue.ToString();
            if (MaxPossibleValue.HasValue && n > MaxPossibleValue) return false;
            if (MinPossibleValue.HasValue && n < MinPossibleValue) return false;

            return true;
        }

        public override object GetValue()
        {
            return dvalue;
        }

        public override void SetValue(object value)
        {
            string msg;
            if (Validate(value, out msg)) dvalue = n;
        }

        public override void SetDefault()
        {
            string msg;
            if (Validate(DefaultValue, out msg)) dvalue = DefaultValue;
            else throw new BadDefaultValueException();
        }

        public override Border GenerateContent()
        {
            Border border = new Border();

            TextBox tb = new TextBox();
            tb.VerticalAlignment = VerticalAlignment.Center;
            tb.Width = 50;
            tb.DataContext = this;
            tb.KeyUp += new KeyEventHandler(tb_MoveFocusOnEnter);

            Binding bind = new Binding();
            bind.Path = new PropertyPath(typeof(DoubleSetting).GetProperties()[0].Name);
            SettingValidationRule svr = new SettingValidationRule(this);
            bind.ValidationRules.Add(svr);
            bind.NotifyOnValidationError = true;
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            tb.SetBinding(TextBox.TextProperty, bind);

            Label l = MakeLabel(Description);

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.Children.Add(l);
            sp.Children.Add(tb);

            border.Margin = Settings.SettingsWindowElementsMargin;
            border.Child = sp;
            return border;
        }
    }


    internal class StringSetting : Setting
    {
        private string _value;

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }
        public string DefaultValue;
        public List<string> PossibleValues;
        public bool AllowEmpty = true;
        public string EmptyErrorMessage = ""; 

        public string n;

        public StringSetting()
        {
            PossibleValues = new List<string>();
        }

        public override bool Validate(object value, out string msg)
        {
            msg = "-";

            if (value is string)
            {
                n = (string)value;
            }
            else
            {
                n = value.ToString();
            }

            if ((n == "" || n == null) && !AllowEmpty)
            {
                if (EmptyErrorMessage == "") msg = "Необходимо значение.";
                else msg = EmptyErrorMessage;
                return false;
            }

            msg = "Возможные варианты:";
            foreach (string i in PossibleValues)
            {
                msg += " " + i.ToString();
            }
            if (PossibleValues.Count > 0)
            {
                if (!PossibleValues.Contains(n)) return false;
            }

            return true;
        }

        public override object GetValue()
        {
            return _value;
        }

        public override void SetValue(object value)
        {
            string msg;
            if (Validate(value, out msg)) _value = n;
        }

        public override void SetDefault()
        {
            string msg;
            if (Validate(DefaultValue, out msg)) _value = DefaultValue;
            else throw new BadDefaultValueException();
        }

        public override Border GenerateContent()
        {
            Border border = new Border();

            Binding bind = new Binding();
            bind.Path = new PropertyPath(typeof(StringSetting).GetProperties()[0].Name);
            SettingValidationRule svr = new SettingValidationRule(this);
            bind.ValidationRules.Add(svr);
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bind.NotifyOnValidationError = true;

            Label l = MakeLabel(Description);

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.Children.Add(l);

            if (PossibleValues.Count > 0)
            {
                ComboBox cb = new ComboBox();
                cb.ItemsSource = PossibleValues;
                cb.SelectedIndex = 0;
                cb.VerticalAlignment = VerticalAlignment.Center;
                cb.Width = 200.0;
                cb.DataContext = this;
                cb.SetBinding(ComboBox.SelectedValueProperty, bind);
                sp.Children.Add(cb);
            }
            else
            {
                TextBox tb = new TextBox();
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.Width = 200.0;
                tb.DataContext = this;
                tb.KeyUp += new KeyEventHandler(tb_MoveFocusOnEnter);
                //tb.TextChanged +=new TextChangedEventHandler(tb_TextChanged);
                tb.SetBinding(TextBox.TextProperty, bind);
                sp.Children.Add(tb);
            }

            border.Margin = Settings.SettingsWindowElementsMargin;
            border.Child = sp;
            return border;
        }
    }


    internal class PathSetting : Setting, INotifyPropertyChanged
    {
        private string _value;

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                NotifyPropertyChanged(typeof(PathSetting).GetProperties()[0].Name);
            }
        }
        public string DefaultValue;
        public bool AllowEmpty = true;
        public bool MustExist = false;


        public string n;

        public PathSetting()
        {
        }

        public override bool Validate(object value, out string msg)
        {
            msg = "-";

            if (value is string)
            {
                n = (string)value;
            }
            else
            {
                n = value.ToString();
            }

            if ((n == "" || n == null) && !AllowEmpty)
            {
                msg = "Неодходимо указать путь.";
                return false;
            }

            if (MustExist && !Directory.Exists(n))
            {
                msg = "Путь указан не верно.";
                return false;
            }

            return true;
        }

        public override object GetValue()
        {
            return _value;
        }

        public override void SetValue(object value)
        {
            string msg;
            if (Validate(value, out msg)) _value = n;
        }

        public override void SetDefault()
        {
            string msg;
            if (Validate(DefaultValue, out msg)) _value = DefaultValue;
            else throw new BadDefaultValueException();
        }

        public override Border GenerateContent()
        {
            Border border = new Border();

            Binding bind = new Binding();
            bind.Path = new PropertyPath(typeof(PathSetting).GetProperties()[0].Name);
            SettingValidationRule svr = new SettingValidationRule(this);
            bind.ValidationRules.Add(svr);
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bind.NotifyOnValidationError = true;

            Label l = MakeLabel(Description);

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.Children.Add(l);

            TextBox tb = new TextBox();
            tb.VerticalAlignment = VerticalAlignment.Center;
            tb.Width = 200.0;
            tb.DataContext = this;
            tb.KeyUp += new KeyEventHandler(tb_MoveFocusOnEnter);
            tb.SetBinding(TextBox.TextProperty, bind);
            sp.Children.Add(tb);

            Button b = new Button();
            b.Content = "Обзор...";
            b.MaxHeight = 24.0;
            b.Margin = new Thickness(1.0);
            b.Padding = new Thickness(3.0, 1.0, 3.0, 1.0);
            b.Click += new RoutedEventHandler(b_Click);
            sp.Children.Add(b);

            border.Margin = Settings.SettingsWindowElementsMargin;
            border.Child = sp;
            return border;
        }

        void b_Click(object sender, RoutedEventArgs e)
        {
            
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();

            System.Windows.Forms.DialogResult result = dlg.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Value = dlg.SelectedPath;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }


    internal class FileSetting : Setting, INotifyPropertyChanged
    {
        private string _value;

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                NotifyPropertyChanged(typeof(FileSetting).GetProperties()[0].Name);
            }
        }
        public string DefaultValue;
        public bool AllowEmpty = true;
        public bool MustExist = false;
        public string DefaultExt = "";
        public string Filter = "";

        public string n;

        public FileSetting()
        {
        }

        public override bool Validate(object value, out string msg)
        {
            msg = "-";

            if (value is string)
            {
                n = (string)value;
            }
            else
            {
                n = value.ToString();
            }

            if ((n == "" || n == null) && !AllowEmpty)
            {
                msg = "Неодходимо указать путь.";
                return false;
            }

            if (MustExist && !File.Exists(n))
            {
                msg = "Путь указан не верно.";
                return false;
            }

            return true;
        }

        public override object GetValue()
        {
            return _value;
        }

        public override void SetValue(object value)
        {
            string msg;
            if (Validate(value, out msg)) _value = n;
        }

        public override void SetDefault()
        {
            string msg;
            if (Validate(DefaultValue, out msg)) _value = DefaultValue;
            else throw new BadDefaultValueException();
        }

        public override Border GenerateContent()
        {
            Border border = new Border();

            Binding bind = new Binding();
            bind.Path = new PropertyPath(typeof(FileSetting).GetProperties()[0].Name);
            SettingValidationRule svr = new SettingValidationRule(this);
            bind.ValidationRules.Add(svr);
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bind.NotifyOnValidationError = true;

            Label l = MakeLabel(Description);

            StackPanel sp = new StackPanel();
            sp.HorizontalAlignment = HorizontalAlignment.Stretch;
            sp.Orientation = Orientation.Horizontal;
            sp.Children.Add(l);

            TextBox tb = new TextBox();
            tb.VerticalAlignment = VerticalAlignment.Center;
            tb.MinWidth = 200.0;
            tb.DataContext = this;
            tb.KeyUp += new KeyEventHandler(tb_MoveFocusOnEnter);
            tb.SetBinding(TextBox.TextProperty, bind);
            sp.Children.Add(tb);

            Button b = new Button();
            b.Content = "Обзор...";
            b.MaxHeight = 24.0;
            b.Margin = new Thickness(1.0);
            b.Padding = new Thickness(3.0, 1.0, 3.0, 1.0);
            b.Click += new RoutedEventHandler(b_Click);
            sp.Children.Add(b);

            border.Margin = Settings.SettingsWindowElementsMargin;
            border.Child = sp;
            return border;
        }

        void b_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.Multiselect = false;
            if (Filter != null) dlg.Filter = Filter;

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                Value = dlg.FileName;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }

    /*
    internal class ColorSetting : Setting, INotifyPropertyChanged
    {
        private Color cvalue;
        private string svalue;

        public string Value
        {
            get
            {
                string msg;
                if (Validate(svalue, out msg) && n == cvalue) return svalue;
                else return cvalue.ToString();
            }
            set
            {
                svalue = value;
                SetValue(value);
            }
        }

        public string DefaultValue;
        public bool AllowEmpty = true;
        public bool MustExist = false;


        public Color n;

        public override bool Validate(object value, out string msg)
        {
            msg = "-";

            if (value is string)
            {
                object o = ColorConverter.ConvertFromString(value as string);
            }
            else if (value is Color)
            {
                n = (Color)value;
            }
            else
            {
                msg = "Не удалость определить цвет.";
                return false;
            }


            return true;
        }

        public override object GetValue()
        {
            return cvalue;
        }

        public override void SetValue(object value)
        {
            string msg;
            if (Validate(value, out msg)) cvalue = n;
        }

        public override void SetDefault()
        {
            string msg;
            if (Validate(DefaultValue, out msg)) cvalue = n;
            else throw new BadDefaultValueException();
        }

        public override Border GenerateContent()
        {
            Border border = new Border();

            Binding bind = new Binding();
            bind.Path = new PropertyPath(typeof(PathSetting).GetProperties()[0].Name);
            SettingValidationRule svr = new SettingValidationRule(this);
            bind.ValidationRules.Add(svr);
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bind.NotifyOnValidationError = true;

            Label l = MakeLabel(Description);

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.Children.Add(l);

            TextBox tb = new TextBox();
            tb.VerticalAlignment = VerticalAlignment.Center;
            tb.Width = 200.0;
            tb.DataContext = this;
            tb.KeyUp += new KeyEventHandler(tb_MoveFocusOnEnter);
            tb.SetBinding(TextBox.TextProperty, bind);
            sp.Children.Add(tb);

            Binding bind2 = new Binding();
            bind2.Path = new PropertyPath(typeof(PathSetting).GetProperties()[0].Name);
            SettingValidationRule svr = new SettingValidationRule(this);
            bind.ValidationRules.Add(svr);
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bind.NotifyOnValidationError = true;

            Button b = new Button();
            b.Content = "Обзор...";
            b.MaxHeight = 24.0;
            b.Margin = new Thickness(1.0);
            b.Padding = new Thickness(3.0, 1.0, 3.0, 1.0);
            b.Click += new RoutedEventHandler(b_Click);
            sp.Children.Add(b);

            border.Margin = Settings.SettingsWindowElementsMargin;
            border.Child = sp;
            return border;
        }

        void b_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();

            System.Windows.Forms.DialogResult result = dlg.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Value = dlg.SelectedPath;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
    */

    internal class MarkupSetting : Setting
    {
        public MarkupType markuptype;
        public string description;

        public MarkupSetting(MarkupType markuptype, string description = "")
        {
            this.markuptype = markuptype;
            this.description = description;
        }

        public override bool Validate(object value, out string msg)
        {
            msg = "-";

            return true;
        }

        public override object GetValue()
        {
            return description;
        }

        public override void SetValue(object value)
        {
        }

        public override void SetDefault()
        {
        }

        public override Border GenerateContent()
        {
            Border border = new Border();

            switch (markuptype)
            {
                case MarkupType.SubCategory:
                    {
                        border.Background = Brushes.WhiteSmoke;
                        border.Margin = new Thickness(3.0, 6.0, 3.0, 3.0);
                        border.CornerRadius = new CornerRadius(2.0);
                        border.BorderBrush = Brushes.DarkGray;
                        border.BorderThickness = new Thickness(0.0, 0.0, 1.0, 1.0);
                        
                        Label l = new Label();
                        l.Content = description;
                        l.FontWeight = FontWeights.Light;
                        border.Child = l;
                    }
                    break;

                case MarkupType.Information:
                    {
                        border.Margin = Settings.SettingsWindowElementsMargin;
                        border.CornerRadius = new CornerRadius(2.0);
                        border.Background = Brushes.GhostWhite;
                        border.BorderThickness = new Thickness(1.0);
                        border.BorderBrush = Brushes.LightBlue;
                        border.SnapsToDevicePixels = true;

                        StackPanel sp = new StackPanel();
                        sp.Orientation = Orientation.Horizontal;

                        Image im = new Image();
                        im.SnapsToDevicePixels = true;
                        im.Source = new BitmapImage(new Uri(@"Images/sign-info-icon.png", UriKind.Relative));
                        im.Stretch = Stretch.None;
                        im.Margin = new Thickness(3.0);
                        im.VerticalAlignment = VerticalAlignment.Top;
                        sp.Children.Add(im);

                        Label l = new Label();
                        l.Content = description;
                        sp.Children.Add(l);

                        border.Child = sp;
                    }
                    break;

                default:
                    {
                        border.Background = Brushes.LightBlue;
                    }
                    break;
            }

            return border;
        }
    }

    internal class SettingValidationRule : ValidationRule
    {
        private Setting setting;
        public SettingValidationRule(Setting setting)
        {
            this.setting = setting;
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string msg;

            if (setting.Validate(value, out msg))
            {
                return new ValidationResult(true, null);
            }
            else
            {
                return new ValidationResult(false, msg);
            }
        }
    }
}
