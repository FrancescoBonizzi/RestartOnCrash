# RestartOnCrash
RestartOnCrash is **very simple** .NET Core application that restarts a given application when not running.

![Icon](RestartOnCrash/_icon.ico)

All you have to do is to configure it with `configuration.json` file and run the application:

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
