using Artemis.Core.Modules;

namespace Artemis.Plugins.Modules.Performance.DataModels
{
    public class PerformanceDataModel : DataModel
    {

        [DataModelProperty(Name = "CPU usage", Affix = "%")]
        public float CpuUsage { get; set; }

        [DataModelProperty(Name = "RAM usage", Affix = "%")]
        public float RamUsage => TotalRam == 0 ? 0 : (TotalRam - AvailableRam) / (float)TotalRam * 100f;

        [DataModelProperty(Name = "Available RAM", Affix = "MB")]
        public long AvailableRam { get; set; }

        [DataModelProperty(Name = "Total RAM", Affix = "MB")]
        public long TotalRam { get; set; }
    }
}