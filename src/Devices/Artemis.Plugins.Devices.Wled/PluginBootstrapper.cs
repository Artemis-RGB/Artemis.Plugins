using Artemis.Core;
using Artemis.Plugins.Devices.Wled.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Devices.Wled;

public class WledBootstrapper : PluginBootstrapper
{
    public override void OnPluginLoaded(Plugin plugin)
    {
        plugin.ConfigurationDialog = new PluginConfigurationDialog<WledConfigurationViewModel>();
    }
}