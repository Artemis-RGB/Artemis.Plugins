using Artemis.Core;
using Artemis.Plugins.Devices.Debug.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Devices.Debug
{
    public class PluginBootstrapper : IPluginBootstrapper
    {
        public void OnPluginLoaded(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<DebugConfigurationViewModel>();
        }

        public void OnPluginEnabled(Plugin plugin)
        {
        }

        public void OnPluginDisabled(Plugin plugin)
        {
        }
    }
}