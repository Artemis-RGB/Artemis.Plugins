using Artemis.Core;
using Artemis.Plugins.Profiling.Prerequisites;
using Artemis.Plugins.Profiling.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Profiling
{
    public class Bootstrapper : PluginBootstrapper
    {
        public override void OnPluginLoaded(Plugin plugin)
        {
            AddFeaturePrerequisite<CpuProfiler>(new DotTracePrerequisite(plugin));
            AddFeaturePrerequisite<MemoryProfiler>(new DotMemoryPrerequisite(plugin));

            plugin.ConfigurationDialog = new PluginConfigurationDialog<ProfilerConfigurationViewModel>();
        }

        public override void OnPluginEnabled(Plugin plugin)
        {
        }

        public override void OnPluginDisabled(Plugin plugin)
        {
        }
    }
}