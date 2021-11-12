using Artemis.Core;
using Artemis.Plugins.Devices.Debug.ViewModels;
using Artemis.UI.Avalonia.Shared;

namespace Artemis.Plugins.Devices.Debug
{
    public class DebugBootstrapper : PluginBootstrapper
    {
        public override void OnPluginLoaded(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<DebugConfigurationViewModel>();
        }
    }
}