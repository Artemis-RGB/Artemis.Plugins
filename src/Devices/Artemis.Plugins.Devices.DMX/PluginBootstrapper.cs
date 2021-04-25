using Artemis.Core;
using Artemis.Plugins.Devices.DMX.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Devices.DMX
{
    public class PluginBootstrapper : IPluginBootstrapper
    {
        public void OnPluginLoaded(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<DMXConfigurationViewModel>();
        }

        public void OnPluginEnabled(Plugin plugin)
        {
        }

        public void OnPluginDisabled(Plugin plugin)
        {
        }
    }
}