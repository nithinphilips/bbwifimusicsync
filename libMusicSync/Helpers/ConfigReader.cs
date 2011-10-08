using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace libMusicSync.Helpers
{
    class ConfigReader
    {
        public static string GetiTunesLibraryPath()
        {
            Configuration config = GetConfigReader();
            if(config != null)
            {
                string value = GetAppSetting(config, "iTunesLibraryPath");
                
                if(!string.IsNullOrWhiteSpace(value)) return value;
            }

            string mymusicDataPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            return Path.Combine(mymusicDataPath, "iTunes", "iTunes Music Library.xml");
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

        static Configuration GetConfigReader()
        {
            Configuration config = null;
            string exeConfigPath = typeof(ConfigReader).Assembly.Location;
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
