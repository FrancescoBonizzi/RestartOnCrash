using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                throw new Exception($"{_configurationFilePath} not found near this application executable");

            var configurationRaw = File.ReadAllText(_configurationFilePath);
            var configuration = JsonConvert.DeserializeObject<Configuration>(configurationRaw);

            if (configuration.PathToApplicationsToMonitor.Length == 0)
                throw new Exception("No applications specified");

            var paths = configuration
                .PathToApplicationsToMonitor
                .Where(path => !string.IsNullOrWhiteSpace(path))
                .Select(Path.GetFullPath)
                .Where(File.Exists)
                .ToArray();

            if (configuration.PathToApplicationsToMonitor.Length == 0)
                throw new Exception("All specified applications path were invalid");

            return configuration with
            {
                PathToApplicationsToMonitor = paths
            };
        }
    }
}
