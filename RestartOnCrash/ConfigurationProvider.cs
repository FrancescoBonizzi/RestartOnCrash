using Newtonsoft.Json;
using System;
using System.IO;

namespace RestartOnCrash
{
    public static class ConfigurationProvider
    {
        private const string _configurationName = "configuration.json";

        public static Configuration Get()
        {
            if (!File.Exists(_configurationName))
            {
                throw new Exception($"{_configurationName} not found near this application executable");
            }

            var configurationRaw = File.ReadAllText(_configurationName);
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
