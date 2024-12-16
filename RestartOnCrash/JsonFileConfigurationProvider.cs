using Newtonsoft.Json;

using System;
using System.IO;
using System.Threading.Tasks;

namespace RestartOnCrash
{
    public class JsonFileConfigurationProvider
    {
        private string _configurationFilePath;

        public JsonFileConfigurationProvider(string configurationFilePath)
        {
            _configurationFilePath = configurationFilePath;
        }

        /// <summary>
        /// Get configuration from configuration.json in async mode.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        async public Task<Configuration> GetAsync()
        {
            if (!File.Exists(_configurationFilePath))
            {
                throw new Exception($"{_configurationFilePath} not found near this application executable");
            }

            var configurationRaw = (await File.ReadAllTextAsync(_configurationFilePath));

            var configuration = JsonConvert.DeserializeObject<Configuration>(configurationRaw)!;

            if (configuration.PathToApplicationToMonitor.Count == 0)
            {
                throw new Exception($"The application to monitor path cannot be null or empty");
            }

            string fullPath = string.Empty;
            for (int i = 0; i < configuration.PathToApplicationToMonitor.Count; i++)
            {
                fullPath = Path.GetFullPath(configuration.PathToApplicationToMonitor[i]);
                if (!File.Exists(fullPath))
                {
                    configuration.PathToApplicationToMonitor.Remove(fullPath);
                    i--;
                }
            }

            return configuration;
        }

        /// <summary>
        /// Get configuration from configuration.json.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Configuration Get()
        {
            if (!File.Exists(_configurationFilePath))
            {
                throw new Exception($"{_configurationFilePath} not found near this application executable");
            }

            var configurationRaw = File.ReadAllText(_configurationFilePath);

            var configuration = JsonConvert.DeserializeObject<Configuration>(configurationRaw)!;

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
                    //continue;
                    configuration.PathToApplicationToMonitor.Remove(fullPath);
                    i--;
                }
            }

            return configuration;
        }
    }
}