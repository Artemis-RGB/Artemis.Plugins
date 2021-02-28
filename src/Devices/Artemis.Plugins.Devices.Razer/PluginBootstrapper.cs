using Artemis.Core;
using Artemis.Plugins.Devices.Razer.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Devices.Razer
{
    public class PluginBootstrapper : IPluginBootstrapper
    {
        public void Enable(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<RazerConfigurationViewModel>();
        }

        public void Disable(Plugin plugin)
        {
        }
    }
}