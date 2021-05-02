using Artemis.Core;
using Artemis.Plugins.Devices.DMX.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Devices.DMX
{
    public class DMXBootstrapper : PluginBootstrapper
    {
        public new void OnPluginLoaded(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<DMXConfigurationViewModel>();
        }
    }
}