using System;
using System.Runtime.Versioning;

namespace Artemis.Plugins.Modules.Time.Platform.Linux
{
    [SupportedOSPlatform("linux")]
    public class LinuxTimeUtils
    {
        public TimeSpan GetTimeSinceSystemStart()
        {
            //TODO: Implement GetTimeSinceSystemStart()
            return TimeSpan.Zero;
        }
    }
}

