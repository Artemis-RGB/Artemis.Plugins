using System;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace Artemis.Plugins.Modules.Time.Platform.Windows;

[SupportedOSPlatform("windows")]
public static class WindowsTimeUtils
{
    public static TimeSpan GetTimeSinceSystemStart()
    {
        using PerformanceCounter uptime = new("System", "System Up Time");
        uptime.NextValue();
        return TimeSpan.FromSeconds(uptime.NextValue());
    }
}