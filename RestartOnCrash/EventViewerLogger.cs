using Serilog;
using System;
using System.Runtime.CompilerServices;

namespace RestartOnCrash
{
    public sealed class EventViewerLogger : IDisposable
    {
        private readonly Serilog.Core.Logger _logger;

        public EventViewerLogger()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.Debug()
                .WriteTo.EventLog(
                    source: "RestartOnCrash",
                    manageEventSource: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }

        public void Dispose()
            => _logger.Dispose();

        public void LogError(Exception ex, [CallerMemberName] string caller = null)
            => _logger.Error(ex, $"{caller}", caller);

        public void LogError(string error, [CallerMemberName] string caller = null)
            => _logger.Error($"{caller}: {error}", caller, error);

        public void LogUnhandledError(Exception ex, [CallerMemberName] string caller = null)
            => _logger.Fatal(ex, $"{caller}", caller);

        public void LogInformation(string message, [CallerMemberName] string caller = null)
            => _logger.Information($"{caller}: {message}", caller, message);

        public void LogWarning(Exception ex, [CallerMemberName] string caller = null)
            => _logger.Warning(ex, $"{caller}", caller);

        public void LogWarning(string error, [CallerMemberName] string caller = null)
            => _logger.Warning($"{caller}: {error}", caller, error);
    }
}
