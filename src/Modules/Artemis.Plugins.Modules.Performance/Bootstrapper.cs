using Artemis.Core;
using Artemis.Plugins.Modules.Performance.Services;
using System;

namespace Artemis.Plugins.Modules.Processes
{
    public class Bootstrapper : PluginBootstrapper
    {
        public override void OnPluginEnabled(Plugin plugin)
        {
            if (OperatingSystem.IsWindows())
                plugin.Kernel!.Bind<IPerformanceService>().To<WindowsPerformanceService>().InSingletonScope();
            else
                throw new NotImplementedException("Platform support not implemented yet");
        }
    }
}