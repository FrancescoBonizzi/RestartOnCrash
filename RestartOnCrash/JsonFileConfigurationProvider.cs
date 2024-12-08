using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;

namespace RestartOnCrash
{
    public class JsonFileConfigurationProvider
    {
        private string _configurationFilePath;

        public JsonFileConfigurationProvider(string configurationFilePath)
        {
            _configurationFilePath = configurationFilePath;
        }

        public Configuration Get()
        {
            if (!File.Exists(_configurationFilePath))
            {
                throw new Exception($"{_configurationFilePath} not found near this application executable");
            }

            var configurationRaw = File.ReadAllText(_configurationFilePath);
            var configuration = JsonConvert.DeserializeObject<Configuration>(configurationRaw);

            if (configuration.PathToApplicationToMonitor.Count == 0)
            {
                throw new Exception($"The application to monitor path cannot be null or empty");
            }
            string fullPath = string.Empty;
            for (int i = 0; i < configuration.PathToApplicationToMonitor.Count; i++)
            {
                fullPath = Path.GetFullPath(configuration.PathToApplicationToMonitor[i]);
                if (File.Exists(fullPath))
                {
                    configuration.PathToApplicationToMonitor[i] = fullPath;
                }
                else
                {
                    configuration.PathToApplicationToMonitor.RemoveAt(i);
                    i--;
                }
            }
            if (!File.Exists(fullPath))
            {
                throw new Exception($"The application at path: {configuration.PathToApplicationToMonitor} does not exists");
            }
            return configuration;
        }
    }
}
