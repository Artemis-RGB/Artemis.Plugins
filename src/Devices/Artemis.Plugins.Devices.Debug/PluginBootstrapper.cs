using Artemis.Core;
using Artemis.Plugins.Devices.Debug.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Devices.Debug
{
    public class DebugBootstrapper : PluginBootstrapper
    {
        public new void OnPluginLoaded(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<DebugConfigurationViewModel>();
        }
    }
}