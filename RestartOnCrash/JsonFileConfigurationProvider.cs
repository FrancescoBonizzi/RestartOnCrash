using Newtonsoft.Json;
using System;
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

            if (string.IsNullOrWhiteSpace(configuration.PathToApplicationToMonitor))
            {
                throw new Exception($"The application to monitor path cannot be null or empty");
            }

            configuration.PathToApplicationToMonitor = Path.GetFullPath(configuration.PathToApplicationToMonitor);
            if (!File.Exists(configuration.PathToApplicationToMonitor))
            {
                throw new Exception($"The application at path: {configuration.PathToApplicationToMonitor} does not exists");
            }

            return configuration;
        }
    }
}
