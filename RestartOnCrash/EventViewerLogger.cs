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
                    manageEventSource: true)
                .CreateLogger();
        }

        public void Dispose()
            => _logger.Dispose();

        public void LogError(Exception ex, [CallerMemberName] string caller = null)
            => _logger.Error($"Caller: {caller}"
                + Environment.NewLine
                + Environment.NewLine
                + ex.ToString());

        public void LogError(string error, [CallerMemberName] string caller = null)
            => _logger.Error($"Caller: {caller}"
                + Environment.NewLine
                + Environment.NewLine
                + error);

        public void LogError(string error, Exception ex, [CallerMemberName] string caller = null)
            => _logger.Error($"Caller: {caller}"
                + Environment.NewLine
                + Environment.NewLine
                + error
                + Environment.NewLine
                + Environment.NewLine
                + ex.ToString());

        public void LogInformation(string message, [CallerMemberName] string caller = null)
            => _logger.Information($"Caller: {caller}"
                + Environment.NewLine
                + Environment.NewLine
                + message);

        public void LogWarning(Exception ex, [CallerMemberName] string caller = null)
            => _logger.Warning($"Caller: {caller}"
                + Environment.NewLine
                + Environment.NewLine
                + ex.ToString());

        public void LogWarning(string warning, [CallerMemberName] string caller = null)
            => _logger.Warning($"Caller: {caller}"
                + Environment.NewLine
                + Environment.NewLine
                + warning);
    }
}
