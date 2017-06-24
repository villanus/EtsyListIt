﻿using System.Configuration;
using EtsyListIt.Utility.Interfaces;

namespace EtsyListIt.Utility
{
    public class SettingsUtility : ISettingsUtility
    {
        public object GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
        

        public void SetAppSetting(string key, string value)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var settings = configFile.AppSettings.Settings;

            if (settings[key] == null || settings[key].ToString().Trim() == string.Empty)
            {
                settings.Add(key, value);
            }
            else
            {
                settings[key].Value = value;
            }

            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }
    }
}