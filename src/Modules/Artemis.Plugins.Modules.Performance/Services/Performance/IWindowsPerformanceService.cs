using Artemis.Plugins.Modules.Performance.Platform.Windows;
using System.Runtime.Versioning;

namespace Artemis.Plugins.Modules.Performance.Services
{
    [SupportedOSPlatform("windows")]
    public class WindowsPerformanceService : IPerformanceService
    {
        public float GetCpuUsage()
        {
            return WindowsPerformance.GetCpuUsage();
        }

        public long GetPhysicalAvailableMemoryInMiB()
        {
            return WindowsPerformance.GetPhysicalAvailableMemoryInMiB();
        }

        public long GetTotalMemoryInMiB()
        {
            return WindowsPerformance.GetTotalMemoryInMiB();
        }
    }
}
