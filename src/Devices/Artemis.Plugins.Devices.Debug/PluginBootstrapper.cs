using Artemis.Core;
using Artemis.Plugins.Devices.Debug.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Devices.Debug
{
    public class PluginBootstrapper : IPluginBootstrapper
    {
        public void Enable(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<DebugConfigurationViewModel>();
        }

        public void Disable(Plugin plugin)
        {
        }
    }
}