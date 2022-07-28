using Artemis.Core;
using Artemis.Plugins.Modules.Processes.Services;
using System;

namespace Artemis.Plugins.Modules.Processes
{
    public class Bootstrapper : PluginBootstrapper
    {
        public override void OnPluginEnabled(Plugin plugin)
        {
            if (OperatingSystem.IsWindows())
                plugin.Kernel!.Bind<IWindowService>().To<WindowsWindowService>().InSingletonScope();
            else
                throw new NotImplementedException("Platform support not implemented yet");
        }
    }
}