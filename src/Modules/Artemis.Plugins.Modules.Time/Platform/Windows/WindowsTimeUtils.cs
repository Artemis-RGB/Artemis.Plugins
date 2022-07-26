using System;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace Artemis.Plugins.Modules.Time.Platform.Windows
{
    [SupportedOSPlatform("windows")]
    public class WindowsTimeUtils
    {
        public static TimeSpan GetTimeSinceSystemStart()
        {
            using var uptime = new PerformanceCounter("System", "System Up Time");
            uptime.NextValue();
            return TimeSpan.FromSeconds(uptime.NextValue());
        }
    }
}
