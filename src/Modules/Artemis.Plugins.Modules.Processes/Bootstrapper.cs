using System;
using Artemis.Core;
using Artemis.Plugins.Modules.Processes.Services.Windows.WindowServices;

namespace Artemis.Plugins.Modules.Processes;

public class Bootstrapper : PluginBootstrapper
{
    public override void OnPluginEnabled(Plugin plugin)
    {
        if (OperatingSystem.IsWindows())
            plugin.Register<IWindowService, WindowsWindowService>();
        else
            throw new NotImplementedException("Platform support not implemented yet");
    }
}