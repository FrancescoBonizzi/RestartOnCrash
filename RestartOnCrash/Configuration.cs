using System;

namespace RestartOnCrash
{
    public class Configuration
    {
        public string PathToApplicationToMonitor { get; set; }
        public TimeSpan CheckInterval { get; set; }
    }
}
