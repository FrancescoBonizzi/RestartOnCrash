using System.Diagnostics;
using System.IO;

namespace RestartOnCrash;

public static class ProcessUtilities
{
    public static bool IsProcessRunning(string processPath)
    {
        var runningProcessByName = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(processPath));
        return runningProcessByName.Length > 0;
    }

    public static bool IsRestartOnCrashRunning()
    {
        var runningProcessByName = Process.GetProcessesByName("RestartOnCrash");

        // One it's me
        return runningProcessByName.Length > 1;
    }
}