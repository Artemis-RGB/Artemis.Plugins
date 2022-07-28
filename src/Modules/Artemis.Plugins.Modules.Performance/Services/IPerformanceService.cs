namespace Artemis.Plugins.Modules.Performance.Services
{
    public interface IPerformanceService
    {
        public float GetCpuUsage();
        public long GetPhysicalAvailableMemoryInMiB();
        public long GetTotalMemoryInMiB();
    }
}
