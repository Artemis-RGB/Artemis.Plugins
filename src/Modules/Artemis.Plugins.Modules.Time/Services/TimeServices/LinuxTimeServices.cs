using Artemis.Plugins.Modules.Time.Platform.Linux;
using System;
using System.Runtime.Versioning;

namespace Artemis.Plugins.Modules.Time.Services.TimeServices
{
    [SupportedOSPlatform("linux")]
    public class LinuxTimeServices : ITimeService
    {
        private readonly LinuxTimeUtils _linuxTimeUtils;

        public LinuxTimeServices()
        {
            _linuxTimeUtils = new LinuxTimeUtils();
        }
        public TimeSpan GetTimeSinceSystemStart()
        {
            return _linuxTimeUtils.GetTimeSinceSystemStart();
        }
    }
}
