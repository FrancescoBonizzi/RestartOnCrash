using System;
using System.Collections.Generic;

namespace RestartOnCrash;

/// <summary>
/// Entity for parsing from configuration.json
/// </summary>
[Serializable]
public record Configuration
{
    /// <summary>
    /// The full path to the application to monitor
    /// </summary>
    public string[] PathToApplicationsToMonitor { get; init; }

    /// <summary>
    /// The check interval for every operation
    /// </summary>
    public TimeSpan CheckInterval { get; init; }

    /// <summary>
    /// If true, the monitored application gets started only if it is already started a first time by its own.
    /// It is useful when you have an application in "startup".
    /// </summary>
    public bool StartApplicationOnlyAfterFirstExecution { get; init; }
}