# RestartOnCrash
RestartOnCrash is **very simple** .NET Core application that restarts a given application when not running.

![Icon](RestartOnCrash/_icon.ico)

All you have to do is to configure it with `configuration.json` file and run the application:

```
{
    "PathToApplicationToMonitor": "C:\\Program Files (x86)\\AnApplicationThatMayCrash.exe",
    "CheckInterval": "00:00:10",
    "StartApplicationOnlyAfterFirstExecution": true
}
```

- `PathToApplicationToMonitor`: self explainatory. At the moment there is the support for just one application
- `CheckInterval`: it is a serialized `TimeSpan`. It represents the interval at which `RestartOnCrash` checks for the target applicaton health
- `StartApplicationOnlyAfterFirstExecution`: if false, when `RestartOnCrash` starts, it starts also the process of the target application; otherwise it waits for the target application to be started the first manually

## TODO (Help is appreciated! XD)
- [Make a simple GUI](/../../issues/1) (Maybe WPF) for `RestartOnCrash`, in order to give the user the possibility to select the target application, to configure the settings
- [Add a tray icon for the application](/../../issues/2) and ensure that just one `RestartOnCrash` is running at time. Add a contextual menù to the tray icon to temporary stop it from restarting the application
- [Make `RestartOnCrash` multi-application ](/../../issues/3)
- Any suggestions? :-)
