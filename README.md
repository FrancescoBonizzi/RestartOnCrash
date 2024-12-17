# RestartOnCrash
RestartOnCrash is **very simple** .NET Core application that restarts a given application when not running.

![Icon](RestartOnCrash/_icon.ico)

All you have to do is to configure it with `configuration.json` file and run the application:

Click "Start service" to start background process for checking all applications in list.
Click "Stop service" to stop background process for checking all applications. It's need to reload you configuration.

- "`Add exe`" open dialog for choosing toy application;
- Select needed app in list and click on "`Del exe`" to remove it;
- "`Restart Period`" requires at least one digit in each place. Like "`0:0:0`";
- "`Wait for first start`" will check application only after you tart it first;

![RestartOnCrashGui](https://github.com/user-attachments/assets/ebf1a907-4398-4e1d-8ae7-91627a8352f3)
```
{
    "PathToApplicationsToMonitor": ["C:\\Program Files (x86)\\AnApplicationThatMayCrash.exe"],
    "CheckInterval": "00:00:10",
    "StartApplicationOnlyAfterFirstExecution": true
}
```

- `PathToApplicationsToMonitor`: self explainatory.
- `CheckInterval`: it is a serialized `TimeSpan`. It represents the interval at which `RestartOnCrash` checks for the target applicaton health
- `StartApplicationOnlyAfterFirstExecution`: if false, when `RestartOnCrash` starts, it starts also the process of the target application; otherwise it waits for the target application to be started the first manually
