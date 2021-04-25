using Artemis.Core;
using Artemis.Plugins.Devices.WS281X.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Devices.WS281X
{
    public class PluginBootstrapper : IPluginBootstrapper
    {
        public void OnPluginLoaded(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<WS281XConfigurationViewModel>();
        }

        public void OnPluginEnabled(Plugin plugin)
        {
        }

        public void OnPluginDisabled(Plugin plugin)
        {
        }
    }
}