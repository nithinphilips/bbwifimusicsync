using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using NotifyPropertyChanged;

namespace MusicSync.Configurator
{
    [NotifyPropertyChanged]
    class SettingsModel
    {
        public int Port { get; set; }
        public string iTunesLibraryPath { get; set; }
        public bool OneWaySync { get; set; }
        public bool AllowPlayerControl { get; set; }

        public void Load()
        {
            var config = GetConfigReader("libMusicSync.dll");
            iTunesLibraryPath = GetAppSetting(config, "iTunesLibraryPath");

            //config = GetConfigReader("MusicSync.Server.exe");
            //int port = 0;
            //if(int.TryParse(GetAppSetting(config, "Port"), out port))
            //{
            //    Port = port;
            //}

            //bool oneWaySync = false;
            //if (bool.TryParse(GetAppSetting(config, "OneWaySync"), out oneWaySync))
            //{
            //    OneWaySync = oneWaySync;
            //}

            //bool allowPlayerControl = false;
            //if(bool.TryParse(GetAppSetting(config, "AllowPlayerControl"), out allowPlayerControl))
            //{
            //    AllowPlayerControl = allowPlayerControl;
            //}
        }

        public void Save()
        {
            var config = GetConfigReader("libMusicSync.dll");
            SetAppSetting(config, "iTunesLibraryPath", iTunesLibraryPath);
            config.Save();
        }

        static string GetAppSetting(Configuration config, string key)
        {
            KeyValueConfigurationElement element = config.AppSettings.Settings[key];
            if (element != null)
            {
                string value = element.Value;
                if (!string.IsNullOrEmpty(value))
                    return value;
            }
            return string.Empty;
        }

        static bool SetAppSetting(Configuration config, string key, string value)
        {
            KeyValueConfigurationElement element = config.AppSettings.Settings[key];
            if (element != null)
            {
                element.Value = value;
                return true;
            }
            return false;
        }

        static Configuration GetConfigReader(string exeConfigPath)
        {
            Configuration config = null;
            try
            {
                config = ConfigurationManager.OpenExeConfiguration(exeConfigPath);
            }
            catch (Exception ex)
            {
                return null;
            }

            return config;
        }
    }
}
