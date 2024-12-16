using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using ThreadState = System.Threading.ThreadState;

namespace RestartOnCrash.UI
{
    /// <summary>
    /// 
    /// </summary>
    partial class MainForm : Form
    {
        const Int16 notSelectedElementIndex = -1;
        const string configurationFileName = "configuration.json";
        bool hasAlreadyStartedManuallyOneTime = false;
        ConfigAttributes programs = new ConfigAttributes() { };
        bool configurationChanged = false;
        CancellationTokenSource cancelTokenSource = new();
        Thread restartOnCrashService = null;

        /// <summary>
        /// 
        /// </summary>
        public class ConfigAttributes
        {
            public string[] PathToApplicationToMonitor { get; set; }
            public List<string> FullPathToApplicationToMonitor { get; set; } = new List<string>();

            private string CheckInterval { get; set; } = "0:15:0";
            private string StartApplicationOnlyAfterFirstExecution { get; set; } = "true";


            /// <summary>
            /// 
            /// </summary>
            /// <param name="time"></param>
            public void SetUserReCheckTimer(TimeOnly time)
            {
                CheckInterval = $"{time.ToShortTimeString()}";
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="time"></param>
            public void SetUserReCheckTimer(string time = "00:00:10")
            {
                CheckInterval = $"{time}";
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public string GetUserReCheckTimer()
            {
                return $"\"CheckInterval\":\"{CheckInterval}\",";
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            public void SetUserSturtupOption(bool value)
            {
                StartApplicationOnlyAfterFirstExecution = $"{value}";
            }

            /// <summary>
            ///
            /// </summary>
            /// <returns></returns>
            public string GetUserSturtupOption()
            {
                string value = StartApplicationOnlyAfterFirstExecution.ToLower();
                return $"\"StartApplicationOnlyAfterFirstExecution\":{value}";
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="allPaths"></param>
            public void SetUserAplicationPathsFromMainForm()
            {
                if (!(FullPathToApplicationToMonitor == null || FullPathToApplicationToMonitor.Count == 0))
                    for (int i = 0; i < FullPathToApplicationToMonitor.Count; i++)
                        PathToApplicationToMonitor[i] = FullPathToApplicationToMonitor[i];
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public string GetApplicationPathsForConfigString()
            {
                string returnValue = string.Empty;
                for (int i = 0; i < PathToApplicationToMonitor.Length; i++)
                    returnValue += $"{Environment.NewLine}\"{PathToApplicationToMonitor[i].Replace("\\", "\\\\")}\",";
                returnValue = returnValue.TrimEnd(',');
                return $"\"PathToApplicationToMonitor\":[{returnValue}\n],";
            }
        }
        private string _filePath = string.Empty;
        private string _fileName = string.Empty;

        public MainForm()
        {
            InitializeComponent();

            const byte correctionIndex = 1;
            int lastIndex = 0;

            selectProgramToCheck.Filter = "|*.exe";
            selectProgramToCheck.DefaultExt = "exe";
            selectProgramToCheck.Title = "Select program to add in List";
            toolTipCheckbox.SetToolTip(
                this.waitBeforeRestart,
                "If true, the monitored application gets started only if it is already started a first time by its own.\n" +
                "It is useful when you have an application in \"startup\".");

            var configurationProvider = new JsonFileConfigurationProvider(configurationFileName);
            var configuration = configurationProvider.Get();
            listOfAddedPrograms.Items.Clear();
            for (int i = 0; i < configuration.PathToApplicationToMonitor.Count; i++)
            {
                string currentElement = configuration.PathToApplicationToMonitor[i];
                programs.FullPathToApplicationToMonitor.Add(currentElement);
                lastIndex = currentElement.LastIndexOf("\\") + correctionIndex;
                listOfAddedPrograms.Items.Add(currentElement[lastIndex..]);
            }
            restartOnCrashService = new(async () => await StartRestartOnCrashService(cancelTokenSource.Token));
        }

        private async void startServiceButton_Click(object sender, EventArgs e)
        {
            using (var logger = new EventViewerLogger())
            {
                if (ProcessUtilities.IsRestartOnCrashRunning())
                {

                    logger.LogWarning("RestartOnCrash is already running, cannot start");
                    ToastService.Notify($"RestartOnCrash is already running, cannot start");
                    return;
                }


                #region [Start Service]
                try
                {
                    if (restartOnCrashService.ThreadState != ThreadState.Unstarted)
                        StopServiceThread_Click(null, null);
                    StartChecherThread();
                }
                catch (Exception ex)
                {
                    ToastService.Notify($"You have exception: {ex}");
                    logger.LogError(ex);
                    // To avoid EventViewer polluting
                    Environment.Exit(-1);
                }
                #endregion
            }
        }

        private void StartChecherThread()
        {
            restartOnCrashService = new(async () => await StartRestartOnCrashService(cancelTokenSource.Token));
            restartOnCrashService.Start();
        }

        async Task StartRestartOnCrashService(CancellationToken cancellationToken)
        {
            var logger = new EventViewerLogger();

            #region [RewriteConfigIfChanged]
            if (configurationChanged)
            {
                var writeConfigsToFileAsync = WriteConfigsToFileAsync();
                await writeConfigsToFileAsync;
            }
            #endregion

            configurationChanged = false;
            var configurationProvider = new JsonFileConfigurationProvider(configurationFileName);
            var configuration = await configurationProvider.GetAsync();
            if (configuration.PathToApplicationToMonitor.Count != 0)
            {
                logger.LogInformation(
                    Environment.NewLine
                    + $"Application to monitor: {configuration.PathToApplicationToMonitor}"
                    + Environment.NewLine
                    + $"Watching every: {Math.Round(configuration.CheckInterval.TotalSeconds, 0)} seconds"
                    + Environment.NewLine
                    + $"{nameof(configuration.StartApplicationOnlyAfterFirstExecution)}: {configuration.StartApplicationOnlyAfterFirstExecution}");

                StartRestartOnCrashService(logger, configuration, cancellationToken: cancellationToken);
            }
            else
            {
                ToastService.Notify("You lis of apps if empty");
            }
        }

        private void StartRestartOnCrashService(EventViewerLogger logger, Configuration configuration, CancellationToken cancellationToken, string currentPath = "")
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                for (int i = 0; i < configuration.PathToApplicationToMonitor.Count; i++)
                {
                    currentPath = configuration.PathToApplicationToMonitor[i];
                    if (!ProcessUtilities.IsProcessRunning(currentPath))
                    {
                        if (!configuration.StartApplicationOnlyAfterFirstExecution || hasAlreadyStartedManuallyOneTime)
                        {
                            logger.LogInformation("Process restarting...");
                            var processInfo = new ProcessStartInfo(currentPath)
                            {
                                // This is very important as if the restarted application searches for assets 
                                // in relative folder, it couldn't find them
                                WorkingDirectory = Path.GetDirectoryName(currentPath)
                            };

                            var process = new Process
                            {
                                StartInfo = processInfo
                            };

                            if (process.Start())
                            {
                                logger.LogInformation($"Process \"{configuration.PathToApplicationToMonitor}\" restarted succesfully!");
                                ToastService.Notify($"\"{Path.GetFileNameWithoutExtension(currentPath)}\" is restarting...");
                            }
                            else
                            {
                                logger.LogError($"Cannot restart \"{configuration.PathToApplicationToMonitor}\"!");
                                ToastService.Notify($"Cannot restart \"{Path.GetFileNameWithoutExtension(currentPath)}\"!");
                            }
                        }
                    }
                    else
                    {
                        hasAlreadyStartedManuallyOneTime = true;
                    }
                }

                Thread.Sleep(configuration.CheckInterval);
            }
        }

        async Task<Task> WriteConfigsToFileAsync()
        {
            string currentFolder = Directory.GetCurrentDirectory();
            var configFile = Directory.GetFiles(currentFolder).First(x => x.Contains(configurationFileName));
            if (configFile == null)
                File.Create(currentFolder + configurationFileName);

            programs.PathToApplicationToMonitor = new string[listOfAddedPrograms.Items.Count];
            programs.SetUserAplicationPathsFromMainForm();
            //using (FileStream fs = File.OpenWrite(configFile))

            string temp = string.Empty;
            using (StreamWriter sw = new StreamWriter(configFile, false))
            {
                try
                {
                    await sw.WriteAsync("");
                    temp = '{' +
                    Environment.NewLine + programs.GetApplicationPathsForConfigString() +
                    Environment.NewLine + programs.GetUserReCheckTimer() +
                    Environment.NewLine + programs.GetUserSturtupOption() +
                    Environment.NewLine + '}';
                }
                catch (Exception ex)
                {
                    ToastService.Notify($"Exception when try to build string to write config file\n{ex}");
                }
                Task fileWriteAsync = sw.WriteAsync(temp);
                sw.Close();
                return fileWriteAsync;
            }
        }

        private void selectFileButton_Click(object sender, EventArgs e)
        {
            if (selectProgramToCheck.ShowDialog() == DialogResult.OK)
            {
                _filePath = selectProgramToCheck.FileName;

                _fileName = selectProgramToCheck.SafeFileName;
                if (listOfAddedPrograms.Items.Contains(_fileName))
                {
                    ToastService.Notify($"{_fileName} already added");
                }
                else
                {
                    listOfAddedPrograms.Items.Add(_fileName);
                    programs.FullPathToApplicationToMonitor.Add(_filePath);
                    ToastService.Notify($"{_fileName} added");
                }

                configurationChanged = true;
            }
        }

        private void StopServiceThread_Click(object sender, EventArgs e)
        {

            if (restartOnCrashService.ThreadState != ThreadState.Unstarted)
            {
                cancelTokenSource.Cancel();
                restartOnCrashService.Join();
                ToastService.Notify("RestartOnCrush service stoped.");
            }
            else
                ToastService.Notify("There's nothing to stop.");
        }


        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
            }
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void removeFileButton_Click(object sender, EventArgs e)
        {
            if (listOfAddedPrograms.Items.Count > 0)
            {
                int removedElement = listOfAddedPrograms.SelectedIndex;
                if (removedElement != notSelectedElementIndex)
                {
                    listOfAddedPrograms.Items.RemoveAt(removedElement);
                    programs.FullPathToApplicationToMonitor.RemoveAt(removedElement);
                }
            }
            configurationChanged = true;
        }

        private void waitBeforeRestart_CheckedChanged(object sender, EventArgs e)
        {
            programs.SetUserSturtupOption(waitBeforeRestart.Checked);
            configurationChanged = true;
        }

        private void timeTextBox_TextChanged(object sender, EventArgs e)
        {
            programs.SetUserReCheckTimer(timeTextBox.Text);
            configurationChanged = true;
        }
    }
}