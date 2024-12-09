using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace RestartOnCrash
{
    public class Program
    {
        private static bool _hasAlreadyStartedManuallyOneTime = false;

        static async Task Main(string[] args)
        {
            var logger = new EventViewerLogger();

            if (ProcessUtilities.IsRestartOnCrashRunning())
            {
                logger.LogWarning("RestartOnCrash is already running, cannot start");
                ToastService.Notify($"RestartOnCrash is already running, cannot start");
                return;
            }

            try
            {
                var configurationProvider = new JsonFileConfigurationProvider("configuration.json");
                var configuration = configurationProvider.Get();

                logger.LogInformation(
                    Environment.NewLine
                    + $"Application to monitor: {configuration.PathToApplicationsToMonitor}"
                    + Environment.NewLine
                    + $"Watching every: {Math.Round(configuration.CheckInterval.TotalSeconds, 0)} seconds"
                    + Environment.NewLine
                    + $"{nameof(configuration.StartApplicationOnlyAfterFirstExecution)}: {configuration.StartApplicationOnlyAfterFirstExecution}");

                while (true)
                {
                    foreach (var currentPath in configuration.PathToApplicationsToMonitor)
                    {
                        if (!ProcessUtilities.IsProcessRunning(currentPath))
                        {
                            if (configuration.StartApplicationOnlyAfterFirstExecution && !_hasAlreadyStartedManuallyOneTime) continue;

                            logger.LogInformation("Process restarting...");
                            var processInfo = new ProcessStartInfo(currentPath)
                            {
                                // This is very important as if the restarted application searches for assets
                                // in relative folder, it couldn't find them
                                WorkingDirectory = Path.GetDirectoryName(currentPath) ?? string.Empty
                            };

                            var process = new Process
                            {
                                StartInfo = processInfo
                            };

                            if (process.Start())
                            {
                                logger.LogInformation($"Process \"{configuration.PathToApplicationsToMonitor}\" restarted succesfully!");
                                ToastService.Notify($"\"{Path.GetFileNameWithoutExtension(currentPath)}\" is restarting...");
                            }
                            else
                            {
                                logger.LogError($"Cannot restart \"{configuration.PathToApplicationsToMonitor}\"!");
                                ToastService.Notify($"Cannot restart \"{Path.GetFileNameWithoutExtension(currentPath)}\"!");
                            }
                        }
                        else
                        {
                            _hasAlreadyStartedManuallyOneTime = true;
                        }
                    }

                    await Task.Delay(configuration.CheckInterval);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                // To avoid EventViewer polluting
                Environment.Exit(-1);
            }
        }
    }
}
