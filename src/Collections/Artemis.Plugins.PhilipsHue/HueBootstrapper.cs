using Artemis.Core;
using Artemis.Plugins.PhilipsHue.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.PhilipsHue;

public class HueBootstrapper : PluginBootstrapper
{
    public override void OnPluginLoaded(Plugin plugin)
    {
        plugin.ConfigurationDialog = new PluginConfigurationDialog<PhilipsHueConfigurationViewModel>();
    }
}