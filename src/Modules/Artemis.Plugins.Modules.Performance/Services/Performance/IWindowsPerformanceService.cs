using System.Runtime.Versioning;
using Artemis.Plugins.Modules.Performance.Platform.Windows;

namespace Artemis.Plugins.Modules.Performance.Services.Performance;

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