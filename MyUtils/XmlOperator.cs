using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using QuadraResources;

namespace MySettings
{
    public static class XmlOperator
    {

        public static void LoadSettings(string filename)
        {
            XDocument doc = new XDocument();

            if (!File.Exists(filename))
            {
                try
                {
                    File.Create(filename).Close();
                }

                catch (Exception ex)
                {
                    string msg = string.Format(ErrorMessages.UnableToCreateFile, filename);
                }
            }

            try
            {
                doc = XDocument.Load(filename);
            }

            catch (Exception ex)
            {
                string msg = string.Format(ErrorMessages.UnableToAccessFile, filename);
            }


            try
            {
                XElement xCategories = doc.Element("Categories");
                foreach (XElement xCategory in xCategories.Elements())
                {
                    //string categoryName = (string)xCategory.Element("Name");
                    string categoryDescription = (string)xCategory.Element("Description");
                    XElement xSettings = xCategory.Element("Settings");

                    foreach (XElement xSetting in xSettings.Elements())
                    {
                        string settingName = (string)xSetting.Element("Name");
                        string settingDescription = (string)xSetting.Element("Description");

                        string settingType = (string)xSetting.Attribute("Type");
                        switch (settingType)
                        {
                            case "Bool":
                                {
                                    bool settingDefault = (bool)xSetting.Element("Default");
                                    Settings.InitBoolSetting(categoryDescription,
                                                             settingName,
                                                             settingDescription,
                                                             settingDefault);

                                    bool settingValue = (bool)xSetting.Element("Value");
                                    Settings.SetValue(settingName, settingValue);
                                    break;
                                }

                            case "Integer":
                                {
                                    int settingDefault = (int)xSetting.Element("Default");
                                    XElement xPossibleValues = xCategory.Element("PossibleValues");
                                    List<int> settingPossibleValues = new List<int>();
                                    int settingMinValue = int.MinValue;
                                    int settingMaxValue = int.MaxValue;

                                    if (xPossibleValues != null)
                                    {
                                        foreach (XElement xPossibleValue in xPossibleValues.Elements())
                                        {
                                            settingPossibleValues.Add((int)xSetting.Element("PossibleValue"));
                                        }
                                    }
                                    else
                                    {
                                        if ((string)xSetting.Element("MinPossibleValue") != "")
                                        {
                                            settingMinValue = (int)xSetting.Element("MinPossibleValue");
                                        }
                                        if ((string)xSetting.Element("MaxPossibleValue") != "")
                                        {
                                            settingMaxValue = (int)xSetting.Element("MaxPossibleValue");
                                        }
                                    }

                                    Settings.InitIntegerSetting(categoryDescription,
                                                            settingName,
                                                            settingDescription,
                                                            settingDefault,
                                                            settingMaxValue,
                                                            settingMinValue,
                                                            settingPossibleValues.ToArray());

                                    int settingValue = (int)xSetting.Element("Value");
                                    Settings.SetValue(settingName, settingValue);
                                    break;
                                }

                            case "Double":
                                {
                                    double settingDefault = (double)xSetting.Element("Default");
                                    XElement xPossibleValues = xCategory.Element("PossibleValues");
                                    List<double> settingPossibleValues = new List<double>();
                                    double settingMinValue = double.MinValue;
                                    double settingMaxValue = double.MaxValue;

                                    if (xPossibleValues != null)
                                    {
                                        foreach (XElement xPossibleValue in xPossibleValues.Elements())
                                        {
                                            settingPossibleValues.Add((double)xSetting.Element("PossibleValue"));
                                        }
                                    }
                                    else
                                    {
                                        if ((string)xSetting.Element("MinPossibleValue") != "")
                                        {
                                            settingMinValue = (double)xSetting.Element("MinPossibleValue");
                                        }
                                        if ((string)xSetting.Element("MaxPossibleValue") != "")
                                        {
                                            settingMaxValue = (double)xSetting.Element("MaxPossibleValue");
                                        }
                                    }

                                    Settings.InitDoubleSetting(categoryDescription,
                                                               settingName,
                                                               settingDescription,
                                                               settingDefault,
                                                               settingMaxValue,
                                                               settingMinValue,
                                                               settingPossibleValues.ToArray());

                                    double settingValue = (double)xSetting.Element("Value");
                                    Settings.SetValue(settingName, settingValue);
                                    break;
                                }

                            case "String":
                                {
                                    string settingDefault = (string)xSetting.Element("Default");
                                    bool settingAllowEmpty = (bool)xSetting.Element("AllowEmpty");
                                    string settingEmptyErrorMessage = (string)xSetting.Element("EmptyErrorMessage");
                                    XElement xPossibleValues = xCategory.Element("PossibleValues");
                                    List<string> settingPossibleValues = new List<string>();

                                    if (xPossibleValues != null)
                                    {
                                        foreach (XElement xPossibleValue in xPossibleValues.Elements())
                                        {
                                            settingPossibleValues.Add((string)xSetting.Element("PossibleValue"));
                                        }
                                    }

                                    Settings.InitStringSetting(categoryDescription,
                                                               settingName,
                                                               settingDescription,
                                                               settingDefault,
                                                               settingAllowEmpty,
                                                               settingEmptyErrorMessage,
                                                               settingPossibleValues.ToArray());

                                    string settingValue = (string)xSetting.Element("Value");
                                    Settings.SetValue(settingName, settingValue);
                                    break;
                                }

                            case "Path":
                                {
                                    string settingDefault = (string)xSetting.Element("Default");
                                    bool settingAllowEmpty = (bool)xSetting.Element("AllowEmpty");
                                    bool settingMustExist = (bool)xSetting.Element("MustExist");

                                    Settings.InitPathSetting(categoryDescription,
                                                             settingName,
                                                             settingDescription,
                                                             settingDefault,
                                                             settingAllowEmpty,
                                                             settingMustExist);

                                    string settingValue = (string)xSetting.Element("Value");
                                    Settings.SetValue(settingName, settingValue);
                                    break;
                                }

                            case "File":
                                {
                                    string settingDefault = (string)xSetting.Element("Default");
                                    bool settingAllowEmpty = (bool)xSetting.Element("AllowEmpty");
                                    bool settingMustExist = (bool)xSetting.Element("MustExist");
                                    string settingDefaultExtension = (string)xSetting.Element("DefaultExtension");
                                    string settingFilter = (string)xSetting.Element("Filter");

                                    Settings.InitFileSetting(categoryDescription,
                                                             settingName,
                                                             settingDescription,
                                                             settingDefault,
                                                             settingAllowEmpty,
                                                             settingMustExist,
                                                             settingFilter);

                                    string settingValue = (string)xSetting.Element("Value");
                                    Settings.SetValue(settingName, settingValue);
                                    break;
                                }

                            case "Markup":
                                {
                                    string settingdescription = (string)xSetting.Element("description");
                                    string settingMarkupTypeString = (string)xSetting.Element("MarkupType");
                                    MarkupType settingMarkupType = MarkupType.Information;
                                    switch (settingMarkupTypeString)
                                    {
                                        case "Information":
                                            settingMarkupType = MarkupType.Information;
                                            break;

                                        case "SubCategory":
                                            settingMarkupType = MarkupType.SubCategory;
                                            break;
                                    }

                                    Settings.InitMarkupSetting(categoryDescription,
                                                               settingMarkupType,
                                                               settingdescription);
                                    break;
                                }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                string msg = String.Format(ErrorMessages.UnableToReadFile, filename);
            }
        }




        public static void SaveSettings(string filename)
        {
            XDocument myXml = new XDocument();

            XElement xCategories = new XElement("Categories");
            myXml.Add(xCategories);

            foreach (SettingsCategory category in Settings.SettingsCategories)
            {
                XElement xCategory = new XElement("Category");
                xCategories.Add(xCategory);
                xCategory.Add(new XElement("Description", category.Description));

                XElement xSettings = new XElement("Settings");
                xCategory.Add(xSettings);

                foreach (Setting setting in category.Settings)
                {
                    XElement xSetting = new XElement("Setting");
                    xSettings.Add(xSetting);
                    xSetting.Add(new XElement("Name", setting.Name));
                    xSetting.Add(new XElement("Description", setting.Description));

                    if (setting is BoolSetting)
                    {
                        xSetting.Add(new XAttribute("Type", "Bool"));
                        xSetting.Add(new XElement("Value", setting.GetValue()));
                        xSetting.Add(new XElement("Default", ((BoolSetting)setting).DefaultValue));
                    }

                    else if (setting is IntegerSetting)
                    {
                        xSetting.Add(new XAttribute("Type", "Integer"));
                        xSetting.Add(new XElement("Value", setting.GetValue()));
                        xSetting.Add(new XElement("Default", ((IntegerSetting)setting).DefaultValue));

                        XElement xPossibleValues = new XElement("PossibleValues");
                        xSetting.Add(xPossibleValues);
                        foreach (int possibleValue in ((IntegerSetting)setting).PossibleValues)
                        {
                            xPossibleValues.Add(new XElement("PossibleValue", possibleValue));
                        }

                        xSetting.Add(new XElement("MinPossibleValue", ((IntegerSetting)setting).MinPossibleValue));
                        xSetting.Add(new XElement("MaxPossibleValue", ((IntegerSetting)setting).MaxPossibleValue));
                    }

                    else if (setting is DoubleSetting)
                    {
                        xSetting.Add(new XAttribute("Type", "Double"));
                        xSetting.Add(new XElement("Value", setting.GetValue()));
                        xSetting.Add(new XElement("Default", ((DoubleSetting)setting).DefaultValue));

                        XElement xPossibleValues = new XElement("PossibleValues");
                        xSetting.Add(xPossibleValues);
                        foreach (double possibleValue in ((DoubleSetting)setting).PossibleValues)
                        {
                            xPossibleValues.Add(new XElement("PossibleValue", possibleValue));
                        }

                        xSetting.Add(new XElement("MinPossibleValue", ((DoubleSetting)setting).MinPossibleValue));
                        xSetting.Add(new XElement("MaxPossibleValue", ((DoubleSetting)setting).MaxPossibleValue));
                    }

                    else if (setting is StringSetting)
                    {
                        xSetting.Add(new XAttribute("Type", "String"));
                        xSetting.Add(new XElement("Value", setting.GetValue()));
                        xSetting.Add(new XElement("Default", ((StringSetting)setting).DefaultValue));

                        XElement xPossibleValues = new XElement("PossibleValues");
                        xSetting.Add(xPossibleValues);
                        foreach (string possibleValue in ((StringSetting)setting).PossibleValues)
                        {
                            xPossibleValues.Add(new XElement("PossibleValue", possibleValue));
                        }

                        xSetting.Add(new XElement("AllowEmpty", ((StringSetting)setting).AllowEmpty));
                        xSetting.Add(new XElement("Default", ((StringSetting)setting).DefaultValue));
                        xSetting.Add(new XElement("EmptyErrorMessage", ((StringSetting)setting).EmptyErrorMessage));
                    }

                    else if (setting is PathSetting)
                    {
                        xSetting.Add(new XAttribute("Type", "Path"));
                        xSetting.Add(new XElement("Value", setting.GetValue()));
                        xSetting.Add(new XElement("Default", ((PathSetting)setting).DefaultValue));
                        xSetting.Add(new XElement("AllowEmpty", ((PathSetting)setting).AllowEmpty));
                        xSetting.Add(new XElement("MustExist", ((PathSetting)setting).MustExist));
                    }

                    else if (setting is FileSetting)
                    {
                        xSetting.Add(new XAttribute("Type", "File"));
                        xSetting.Add(new XElement("Value", setting.GetValue()));
                        xSetting.Add(new XElement("Default", ((FileSetting)setting).DefaultValue));
                        xSetting.Add(new XElement("AllowEmpty", ((FileSetting)setting).AllowEmpty));
                        xSetting.Add(new XElement("MustExist", ((FileSetting)setting).MustExist));
                        xSetting.Add(new XElement("DefaultExtension", ((FileSetting)setting).DefaultExt));
                        xSetting.Add(new XElement("Filter", ((FileSetting)setting).Filter));
                    }

                    else if (setting is MarkupSetting)
                    {
                        xSetting.Add(new XAttribute("Type", "Markup"));
                        xSetting.Add(new XElement("MarkupType", ((MarkupSetting)setting).markuptype));
                        xSetting.Add(new XElement("description", ((MarkupSetting)setting).description));
                    }
                }
            }

            try
            {
                myXml.Save(filename);
            }

            catch (Exception ex)
            {
                string msg = String.Format(ErrorMessages.UnableToSaveFile, filename);
            }
        }
    }
}

