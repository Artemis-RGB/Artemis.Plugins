using System;
using System.Collections.Generic;
using Artemis.Core;
using Artemis.Core.Modules;
using Artemis.Plugins.Modules.Performance.DataModels;
using Artemis.Plugins.Modules.Performance.Services;

namespace Artemis.Plugins.Modules.Performance;

[PluginFeature(Name = "Performance", AlwaysEnabled = true)]
public class PerformanceModule : Module<PerformanceDataModel>
{
    private readonly IPerformanceService _performanceService;

    public PerformanceModule(IPerformanceService performanceService)
    {
        _performanceService = performanceService;
    }

    public override List<IModuleActivationRequirement> ActivationRequirements => null;

    public override void Enable()
    {
        //TODO: Make update frequency configurable
        AddTimedUpdate(TimeSpan.FromMilliseconds(100), _ => UpdatePerformance(), "UpdatePerformance");
    }

    public override void Disable()
    {
    }

    public override void Update(double deltaTime)
    {
    }

    private void UpdatePerformance()
    {
        // Performance counters are slow, only update them if necessary
        if (IsPropertyInUse("CpuUsage", false))
            DataModel.CpuUsage = _performanceService.GetCpuUsage();
        if (IsPropertyInUse("AvailableRam", false) || IsPropertyInUse("RamUsage", false))
            DataModel.AvailableRam = _performanceService.GetPhysicalAvailableMemoryInMiB();
        if (IsPropertyInUse("TotalRam", false) || IsPropertyInUse("RamUsage", false))
            DataModel.TotalRam = _performanceService.GetTotalMemoryInMiB();
    }
}
