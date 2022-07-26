using Artemis.Plugins.Modules.Time.Platform.Windows;
using System;
using System.Runtime.Versioning;

namespace Artemis.Plugins.Modules.Time.Services.TimeServices
{
    [SupportedOSPlatform("windows")]
    public class WindowsTimeServices : ITimeService
    {
        public TimeSpan GetTimeSinceSystemStart()
        {
            return WindowsTimeUtils.GetTimeSinceSystemStart();
        }
    }
}
