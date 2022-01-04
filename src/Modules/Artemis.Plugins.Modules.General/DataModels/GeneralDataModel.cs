using System;
using System.Collections.Generic;
using Artemis.Core.Modules;
using Artemis.Plugins.Modules.General.DataModels.Windows;
using SkiaSharp;

namespace Artemis.Plugins.Modules.General.DataModels
{
    public class GeneralDataModel : DataModel
    {
        public GeneralDataModel()
        {
            TimeDataModel = new TimeDataModel();
            PerformanceDataModel = new PerformanceDataModel();
        }

        public WindowDataModel ActiveWindow { get; set; }

        [DataModelProperty(ListItemName = "Process name")]
        public List<string> RunningProcesses { get; set; }

        [DataModelProperty(Name = "Time")]
        public TimeDataModel TimeDataModel { get; set; }
        [DataModelProperty(Name = "Performance")]
        public PerformanceDataModel PerformanceDataModel { get; set; }
    }

    public class TimeDataModel : DataModel
    {
        [DataModelProperty(Name = "Current")]
        public DateTimeOffset CurrentTime { get; set; }
        public TimeSpan TimeSinceMidnight { get; set; }
    }

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