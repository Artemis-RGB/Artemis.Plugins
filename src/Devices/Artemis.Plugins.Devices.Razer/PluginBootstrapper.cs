using Artemis.Core;
using Artemis.Plugins.Devices.Razer.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Devices.Razer
{
    public class RazerBootstrapper : PluginBootstrapper
    {
        public override void OnPluginLoaded(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<RazerConfigurationViewModel>();
        }
    }
}