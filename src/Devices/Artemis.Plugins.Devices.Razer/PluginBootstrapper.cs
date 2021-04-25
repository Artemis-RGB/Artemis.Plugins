using Artemis.Core;
using Artemis.Plugins.Devices.Razer.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Devices.Razer
{
    public class PluginBootstrapper : IPluginBootstrapper
    {
        public void OnPluginLoaded(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<RazerConfigurationViewModel>();
        }

        public void OnPluginEnabled(Plugin plugin)
        {
        }

        public void OnPluginDisabled(Plugin plugin)
        {
        }
    }
}