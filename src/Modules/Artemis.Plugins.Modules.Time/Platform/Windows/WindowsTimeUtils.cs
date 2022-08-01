using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Artemis.Plugins.Modules.Time.Platform.Windows;

[SupportedOSPlatform("windows")]
public static class WindowsTimeUtils
{
    [DllImport("kernel32.dll")]
    static extern long GetTickCount64();
    
    public static TimeSpan GetTimeSinceSystemStart()
    {
        return new TimeSpan(10000 * GetTickCount64());
    }
}