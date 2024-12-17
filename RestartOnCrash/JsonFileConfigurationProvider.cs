using Newtonsoft.Json;

using System;
using System.IO;
using System.Linq;
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
                throw new Exception($"{_configurationFilePath} not found near this application executable");

            var configurationRaw = (await File.ReadAllTextAsync(_configurationFilePath));
            var configuration = JsonConvert.DeserializeObject<Configuration>(configurationRaw)!;

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

        /// <summary>
        /// Get configuration from configuration.json.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Configuration Get()
        {
            if (!File.Exists(_configurationFilePath))
                throw new Exception($"{_configurationFilePath} not found near this application executable");

            var configurationRaw = File.ReadAllText(_configurationFilePath);

            var configuration = JsonConvert.DeserializeObject<Configuration>(configurationRaw)!;
            if (configuration.PathToApplicationsToMonitor != null)
                if (configuration.PathToApplicationsToMonitor.Length != 0)
                {

                    string fullPath = string.Empty;
                    for (int i = 0; i < configuration.PathToApplicationsToMonitor.Length; i++)
                    {
                        fullPath = Path.GetFullPath(configuration.PathToApplicationsToMonitor[i]);
                        if (File.Exists(fullPath))
                        {
                            configuration.PathToApplicationsToMonitor[i] = fullPath;
                        }
                        else
                        {
                            //continue;
                            configuration.PathToApplicationsToMonitor = configuration.PathToApplicationsToMonitor.Where(removedElement =>removedElement != fullPath).ToArray();
                            i--;
                        }
                    }
                }
            return configuration;
        }
    }
}