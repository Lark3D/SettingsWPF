using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows;

namespace MySettings
{
    public static class Settings
    {
        internal static Thickness SettingsWindowElementsMargin = new Thickness(23.0, 3.0, 3.0, 3.0);

        static string settingsfilename = "";
        static List<SettingsCategory> settingscategories = new List<SettingsCategory>();
        static string settingswindowtitle = "Параметры";
        static double labelswidth = 0.0;
        
        public static string SettingsFileName
        {
            get { return settingsfilename; }
            set { settingsfilename = value; }
        }

        internal static List<SettingsCategory> SettingsCategories
        {
            get { return settingscategories; }
            set { settingscategories = value; }
        }

        public static string SettingsWindowTitle
        {
            get { return settingswindowtitle; }
            set { settingswindowtitle = value; }
        }

        public static double LabelsWidth
        {
            get { return labelswidth; }
            set { labelswidth = value; }
        }



        public static object GetValue(string settingname)
        {
            object o = null;

            foreach (SettingsCategory sc in SettingsCategories)
                foreach (Setting s in sc.Settings)
                    if (s.Name == settingname) return s.GetValue();

            return o;
        }

        public static void SetValue(string settingname, object value)
        {

            foreach (SettingsCategory sc in SettingsCategories)
                foreach (Setting s in sc.Settings)
                    if (s.Name == settingname)
                    {
                        s.SetValue(value);
                        return;
                    }
        }

        public static void ShowSettingsWindow(Window owner = null)
        {
            Save();
            SettingsWindow sw = new SettingsWindow();
            sw.Title = SettingsWindowTitle;
            if (owner != null)
            {
                sw.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                sw.Owner = owner;
            }
            sw.ShowDialog();
            sw.Close();
        }

        public static void Load()
        {

            foreach (SettingsCategory sc in SettingsCategories)
            {
                foreach (Setting s in sc.Settings)
                    s.SetDefault();
            }

            if (!File.Exists(SettingsFileName)) return;

            try
            {
                XmlTextReader xr = new XmlTextReader(SettingsFileName);
                while (xr.Read())
                {
                    if (xr.Name == "Setting")
                    {
                        string sname = xr.GetAttribute("Name");
                        string svalue = xr.GetAttribute("Value");
                        SetValue(sname, svalue);
                    }
                }
                xr.Close();
            }
            catch
            {
            }

        }

        public static void Save()
        {
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            XmlWriter xw = XmlWriter.Create(SettingsFileName, xws);

            xw.WriteStartDocument();
            xw.WriteComment(" Файл настроек. Не рекомендуется изменять вручную. ");
            xw.WriteStartElement("Settings");

            foreach (SettingsCategory sc in SettingsCategories)
            {
                xw.WriteStartElement("Category");
                xw.WriteAttributeString("Description", sc.Description);

                foreach (Setting s in sc.Settings)
                {
                    if (s is MarkupSetting) continue;
                    xw.WriteComment(s.Description);
                    xw.WriteStartElement("Setting");
                    xw.WriteAttributeString("Name", s.Name);
                    xw.WriteAttributeString("Value", s.GetValue().ToString());
                    xw.WriteEndElement();
                }

                xw.WriteEndElement();
            }
            xw.WriteEndElement();
            xw.WriteEndDocument();
            xw.Close();
        }

        public static void InitMarkupSetting(string catname, MarkupType markuptype, string description)
        {
            MarkupSetting s = new MarkupSetting(markuptype, description);

            GetSettingsCategory(catname).Settings.Add(s);
        }

        public static void InitBoolSetting(string catname, string setname, string description, bool defaultvalue)
        {
            if (HasSetting(setname)) throw new NameConflictException();

            BoolSetting s = new BoolSetting();

            s.Name = setname;
            s.Description = description;
            s.DefaultValue = defaultvalue;

            GetSettingsCategory(catname).Settings.Add(s);
        }

        public static void InitIntegerSetting(string catname, string setname, string description, int defaultvalue, int? max, int? min, params int[] possiblevalues)
        {
            if (HasSetting(setname)) throw new NameConflictException();

            IntegerSetting s = new IntegerSetting();

            s.Name = setname;
            s.Description = description;
            s.DefaultValue = defaultvalue;
            s.MaxPosibleValue = max;
            s.MinPosibleValue = min;
            s.PossibleValues.AddRange(possiblevalues);
            
            GetSettingsCategory(catname).Settings.Add(s);
        }

        public static void InitDoubleSetting(string catname, string setname, string description, double defaultvalue, double? max, double? min, params double[] possiblevalues)
        {
            if (HasSetting(setname)) throw new NameConflictException();

            DoubleSetting s = new DoubleSetting();

            s.Name = setname;
            s.Description = description;
            s.DefaultValue = defaultvalue;
            s.MaxPosibleValue = max;
            s.MinPosibleValue = min;
            s.PossibleValues.AddRange(possiblevalues);

            GetSettingsCategory(catname).Settings.Add(s);
        }

        public static void InitStringSetting(string catname, string setname, string description, string defaultvalue, bool allowempty, string erroremptymsg, params string[] possiblevalues)
        {
            if (HasSetting(setname)) throw new NameConflictException();

            StringSetting s = new StringSetting();

            s.Name = setname;
            s.Description = description;
            s.DefaultValue = defaultvalue;
            s.AllowEmpty = allowempty;
            s.EmptyErrorMessage = erroremptymsg;
            s.PossibleValues.AddRange(possiblevalues);

            GetSettingsCategory(catname).Settings.Add(s);
        }

        public static void InitPathSetting(string catname, string setname, string description, string defaultvalue, bool allowempty, bool mustexist)
        {
            if (HasSetting(setname)) throw new NameConflictException();

            PathSetting s = new PathSetting();

            s.Name = setname;
            s.Description = description;
            s.DefaultValue = defaultvalue;
            s.AllowEmpty = allowempty;
            s.MustExist = mustexist;

            GetSettingsCategory(catname).Settings.Add(s);
        }

        public static void InitFileSetting(string catname, string setname, string description, string defaultvalue, bool allowempty, bool mustexist, string filter)
        {
            if (HasSetting(setname)) throw new NameConflictException();

            FileSetting s = new FileSetting();

            s.Name = setname;
            s.Description = description;
            s.DefaultValue = defaultvalue;
            s.AllowEmpty = allowempty;
            s.MustExist = mustexist;
            s.Filter = filter;

            GetSettingsCategory(catname).Settings.Add(s);
        }

        

        static SettingsCategory GetSettingsCategory(string catname)
        {
            for (int i = 0; i < SettingsCategories.Count; i++)
            {
                if (SettingsCategories[i].Description == catname) return SettingsCategories[i];
            }

            SettingsCategory sc = new SettingsCategory(catname);
            SettingsCategories.Add(sc);
            return sc;
        }

        static bool HasSetting(string setname)
        {
            bool flag = false;

            foreach (SettingsCategory sc in SettingsCategories)
            {
                foreach (Setting s in sc.Settings)
                {
                    if (s.Name == setname)
                    {
                        flag = true;
                    }
                }
            }

            return flag;
        }

    }

    public enum MarkupType
    {
        SubCategory,
        Information
    }

#region Исключения

    public class BadDefaultValueException : Exception
    {
        public BadDefaultValueException()
            : base("Умолчательное значение не проходит валидацию.")
        {
        }
    }

    public class NameConflictException : Exception
    {
        public NameConflictException()
            : base("Имя инициализированного свойства совпадает с уже существующим.")
        {
        }
    }

#endregion

}
