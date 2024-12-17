using Newtonsoft.Json;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace RestartOnCrash;

public class JsonFileConfigurationProvider
{
    private readonly string _configurationFilePath;

    public JsonFileConfigurationProvider(string configurationFilePath)
    {
        _configurationFilePath = configurationFilePath;
    }

    /// <summary>
    /// Get configuration from configuration.json in async mode.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<Configuration> GetAsync()
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
        if (configuration.PathToApplicationsToMonitor == null)
            return configuration;

        if (configuration.PathToApplicationsToMonitor.Length == 0)
            return configuration;

        var existingPaths = configuration
            .PathToApplicationsToMonitor
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Where(path => File.Exists(Path.GetFullPath(path)))
            .Select(Path.GetFullPath)
            .ToArray();

        return configuration with
        {
            PathToApplicationsToMonitor = existingPaths
        };
    }
}