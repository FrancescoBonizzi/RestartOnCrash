using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
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
                var configuration = ConfigurationProvider.Get();

                logger.LogInformation($"Application to monitor every " +
                    $"{Math.Round(configuration.CheckInterval.TotalSeconds, 0)} seconds: " +
                    $"{configuration.PathToApplicationToMonitor}");

                while (true)
                {
                    if (!ProcessUtilities.IsProcessRunning(configuration.PathToApplicationToMonitor))
                    {
                        if (!configuration.StartApplicationOnlyAfterFirstExecution || _hasAlreadyStartedManuallyOneTime)
                        {
                            logger.LogInformation("Process restarting...");
                            var processInfo = new ProcessStartInfo(configuration.PathToApplicationToMonitor);
                            var process = new Process
                            {
                                StartInfo = processInfo
                            };

                            if (process.Start())
                            {
                                logger.LogInformation($"Process \"{configuration.PathToApplicationToMonitor}\" restarted succesfully!");
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
