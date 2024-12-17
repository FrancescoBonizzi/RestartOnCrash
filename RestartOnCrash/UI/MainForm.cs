using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreadState = System.Threading.ThreadState;

namespace RestartOnCrash.UI;

partial class MainForm : Form
{
    private const short NotSelectedElementIndex = -1;
    private const string ConfigurationFileName = "configuration.json";
    private bool _hasAlreadyStartedManuallyOneTime;
    private bool _configurationChanged;

    private readonly CancellationTokenSource _cancelTokenSource = new();
    private readonly ConfigAttributes _programs = new();
    private Thread _restartOnCrashService;

    public class ConfigAttributes
    {
        public string[] PathToApplicationToMonitor { get; set; }
        public List<string> FullPathToApplicationToMonitor { get; set; } = [];

        private string CheckInterval { get; set; } = "0:20:0";
        private string StartApplicationOnlyAfterFirstExecution { get; set; } = "true";

        public void SetUserReCheckTimer(TimeOnly time)
        {
            CheckInterval = $"{time.ToShortTimeString()}";
        }

        public void SetUserReCheckTimer(string time = "00:00:20")
        {
            CheckInterval = $"{time}";
        }

        public string GetUserReCheckTimer()
        {
            return $"\"CheckInterval\":\"{CheckInterval}\",";
        }

        public void SetUserSturtupOption(bool value)
        {
            StartApplicationOnlyAfterFirstExecution = $"{value}";
        }

        public string GetUserSturtupOption()
        {
            string value = StartApplicationOnlyAfterFirstExecution.ToLower();
            return $"\"StartApplicationOnlyAfterFirstExecution\":{value}";
        }

        public void SetUserAplicationPathsFromMainForm()
        {
            if (FullPathToApplicationToMonitor == null || FullPathToApplicationToMonitor.Count == 0)
                return;

            for (var i = 0; i < FullPathToApplicationToMonitor.Count; i++)
                PathToApplicationToMonitor[i] = FullPathToApplicationToMonitor[i];
        }

        public string GetApplicationPathsForConfigString()
        {
            var returnValue = string.Empty;

            foreach (var t in PathToApplicationToMonitor)
                returnValue += $"{Environment.NewLine}\"{t.Replace("\\", "\\\\")}\",";

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

        selectProgramToCheck.Filter = "|*.exe";
        selectProgramToCheck.DefaultExt = "exe";
        selectProgramToCheck.Title = "Select program to add in List";
        toolTipCheckbox.SetToolTip(
            this.waitBeforeRestart,
            "If true, the monitored application gets started only if it is already started a first time by its own.\n" +
            "It is useful when you have an application in \"startup\".");

        var configurationProvider = new JsonFileConfigurationProvider(ConfigurationFileName);
        var configuration = configurationProvider.Get();
        listOfAddedPrograms.Items.Clear();
        if (configuration.PathToApplicationsToMonitor != null)
            foreach (var currentElement in configuration.PathToApplicationsToMonitor)
            {
                _programs.FullPathToApplicationToMonitor.Add(currentElement);
                var lastIndex = currentElement.LastIndexOf('\\') + correctionIndex;
                listOfAddedPrograms.Items.Add(currentElement[lastIndex..]);
            }

        _restartOnCrashService = new Thread(async void () =>
        {
            try
            {
                await StartRestartOnCrashService(_cancelTokenSource.Token);
            }
            catch (Exception ex)
            {
                using var logger = new EventViewerLogger();
                logger.LogError(ex);
            }
        });
    }

    private void startServiceButton_Click(object sender, EventArgs e)
    {
        using var logger = new EventViewerLogger();

        if (ProcessUtilities.IsRestartOnCrashRunning())
        {
            logger.LogWarning("RestartOnCrash is already running, cannot start");
            ToastService.Notify($"RestartOnCrash is already running, cannot start");
            return;
        }

        #region [Start Service]

        try
        {
            if (_restartOnCrashService.ThreadState != ThreadState.Unstarted)
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

    private void StartChecherThread()
    {
        _restartOnCrashService = new(async () => await StartRestartOnCrashService(_cancelTokenSource.Token));
        _restartOnCrashService.Start();
    }

    private async Task StartRestartOnCrashService(CancellationToken cancellationToken)
    {
        var logger = new EventViewerLogger();

        #region [RewriteConfigIfChanged]

        if (_configurationChanged)
        {
            var writeConfigsToFileAsync = WriteConfigsToFileAsync();
            await writeConfigsToFileAsync;
        }

        #endregion

        _configurationChanged = false;
        var configurationProvider = new JsonFileConfigurationProvider(ConfigurationFileName);
        var configuration = await configurationProvider.GetAsync();
        if (configuration.PathToApplicationsToMonitor.Length != 0)
        {
            logger.LogInformation(
                Environment.NewLine
                + $"Application to monitor: {configuration.PathToApplicationsToMonitor}"
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

    private void StartRestartOnCrashService(EventViewerLogger logger, Configuration configuration,
        CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            foreach (var currentPath in configuration.PathToApplicationsToMonitor)
            {
                if (!ProcessUtilities.IsProcessRunning(currentPath))
                {
                    if (configuration.StartApplicationOnlyAfterFirstExecution &&
                        !_hasAlreadyStartedManuallyOneTime) continue;

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
                        logger.LogInformation(
                            $"Process \"{configuration.PathToApplicationsToMonitor}\" restarted succesfully!");
                        ToastService.Notify(
                            $"\"{Path.GetFileNameWithoutExtension(currentPath)}\" is restarting...");
                    }
                    else
                    {
                        logger.LogError($"Cannot restart \"{configuration.PathToApplicationsToMonitor}\"!");
                        ToastService.Notify(
                            $"Cannot restart \"{Path.GetFileNameWithoutExtension(currentPath)}\"!");
                    }
                }
                else
                {
                    _hasAlreadyStartedManuallyOneTime = true;
                }
            }

            Thread.Sleep(configuration.CheckInterval);
        }
    }

    private async Task<Task> WriteConfigsToFileAsync()
    {
        var currentFolder = Directory.GetCurrentDirectory();
        var configFile = Directory.GetFiles(currentFolder).First(x => x.Contains(ConfigurationFileName));

        if (string.IsNullOrWhiteSpace(configFile))
        {
            File.Create(currentFolder + ConfigurationFileName);
            configFile = Directory.GetFiles(currentFolder).First(x => x.Contains(ConfigurationFileName));
        }

        _programs.PathToApplicationToMonitor = new string[listOfAddedPrograms.Items.Count];
        _programs.SetUserAplicationPathsFromMainForm();

        var temp = string.Empty;

        await using var sw = new StreamWriter(configFile, false);

        try
        {
            await sw.WriteAsync("");
            temp = '{' +
                   Environment.NewLine + _programs.GetApplicationPathsForConfigString() +
                   Environment.NewLine + _programs.GetUserReCheckTimer() +
                   Environment.NewLine + _programs.GetUserSturtupOption() +
                   Environment.NewLine + '}';
        }
        catch (Exception ex)
        {
            ToastService.Notify($"Exception when try to build string to write config file\n{ex}");
        }

        var fileWriteAsync = sw.WriteAsync(temp);
        sw.Close();

        return fileWriteAsync;
    }

    private void selectFileButton_Click(object sender, EventArgs e)
    {
        if (selectProgramToCheck.ShowDialog() != DialogResult.OK)
            return;

        _filePath = selectProgramToCheck.FileName;

        _fileName = selectProgramToCheck.SafeFileName;
        if (listOfAddedPrograms.Items.Contains(_fileName))
        {
            ToastService.Notify($"{_fileName} already added");
        }
        else
        {
            listOfAddedPrograms.Items.Add(_fileName);
            _programs.FullPathToApplicationToMonitor.Add(_filePath);
            ToastService.Notify($"{_fileName} added");
        }

        _configurationChanged = true;
    }

    private void StopServiceThread_Click(object sender, EventArgs e)
    {
        if (_restartOnCrashService.ThreadState != ThreadState.Unstarted)
        {
            _cancelTokenSource.Cancel();
            _restartOnCrashService.Join();
            ToastService.Notify("RestartOnCrush service stoped.");
        }
        else
            ToastService.Notify("There's nothing to stop.");
    }


    private void MainForm_Resize(object sender, EventArgs e)
    {
        if (WindowState != FormWindowState.Minimized)
            return;

        Hide();
        notifyIcon.Visible = true;
    }

    private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
    {
        Show();
        WindowState = FormWindowState.Normal;
        notifyIcon.Visible = false;
    }

    private void removeFileButton_Click(object sender, EventArgs e)
    {
        if (listOfAddedPrograms.Items.Count > 0)
        {
            var removedElement = listOfAddedPrograms.SelectedIndex;
            if (removedElement != NotSelectedElementIndex)
            {
                listOfAddedPrograms.Items.RemoveAt(removedElement);
                _programs.FullPathToApplicationToMonitor.RemoveAt(removedElement);
            }
        }

        _configurationChanged = true;
    }

    private void waitBeforeRestart_CheckedChanged(object sender, EventArgs e)
    {
        _programs.SetUserSturtupOption(waitBeforeRestart.Checked);
        _configurationChanged = true;
    }

    private void timeTextBox_TextChanged(object sender, EventArgs e)
    {
        _programs.SetUserReCheckTimer(timeTextBox.Text);
        _configurationChanged = true;
    }
}