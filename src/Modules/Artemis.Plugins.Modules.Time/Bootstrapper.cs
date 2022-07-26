using Artemis.Core;
using Artemis.Plugins.Modules.Time.Services.TimeServices;
using System;

namespace Artemis.Plugins.Modules.Time
{
    public class Bootstrapper : PluginBootstrapper
    {
        public override void OnPluginEnabled(Plugin plugin)
        {
            if (OperatingSystem.IsWindows())
                plugin.Kernel!.Bind<ITimeService>().To<WindowsTimeServices>().InSingletonScope();
            else if (OperatingSystem.IsLinux())
                plugin.Kernel!.Bind<ITimeService>().To<LinuxTimeServices>().InSingletonScope();
        }
    }
}
