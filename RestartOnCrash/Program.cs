using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace RestartOnCrash
{
    public class Program
    {
        private static bool _hasAlreadyStartedManuallyOneTime = false;

        async static Task Main(string[] args)
        {
            var logger = new EventViewerLogger();

            if (ProcessUtilities.IsRestartOnCrashRunning())
            {
                logger.LogWarning("RestartOnCrash is already running, cannot start");
                return;
            }

            try
            {
                var configurationProvider = new JsonFileConfigurationProvider("configuration.json");
                var configuration = configurationProvider.Get();

                logger.LogInformation(
                    Environment.NewLine
                    + $"Application to monitor: {configuration.PathToApplicationToMonitor}"
                    + Environment.NewLine
                    + $"Watching every: {Math.Round(configuration.CheckInterval.TotalSeconds, 0)} seconds"
                    + Environment.NewLine
                    + $"{nameof(configuration.StartApplicationOnlyAfterFirstExecution)}: {configuration.StartApplicationOnlyAfterFirstExecution}");

                while (true)
                {
                    if (!ProcessUtilities.IsProcessRunning(configuration.PathToApplicationToMonitor))
                    {
                        if (!configuration.StartApplicationOnlyAfterFirstExecution || _hasAlreadyStartedManuallyOneTime)
                        {
                            logger.LogInformation("Process restarting...");
                            var processInfo = new ProcessStartInfo(configuration.PathToApplicationToMonitor)
                            {
                                // This is very important as if the restarted application searches for assets 
                                // in relative folder, it couldn't find them
                                WorkingDirectory = Path.GetDirectoryName(configuration.PathToApplicationToMonitor)
                            };

                            var process = new Process
                            {
                                StartInfo = processInfo
                            };

                            if (process.Start())
                            {
                                logger.LogInformation($"Process \"{configuration.PathToApplicationToMonitor}\" restarted succesfully!");
                                ToastService.Notify($" \"{Path.GetFileNameWithoutExtension(configuration.PathToApplicationToMonitor)}\" is restarting...");
                            }
                            else
                            {
                                logger.LogError($"Cannot restart \"{configuration.PathToApplicationToMonitor}\"!");
                            }
                        }
                    }
                    else
                    {
                        _hasAlreadyStartedManuallyOneTime = true;
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
