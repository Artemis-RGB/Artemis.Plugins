using Artemis.Core;
using Artemis.Plugins.Devices.WS281X.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Devices.WS281X
{
    public class PluginBootstrapper : IPluginBootstrapper
    {
        public void Enable(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<WS281XConfigurationViewModel>();
        }

        public void Disable(Plugin plugin)
        {
        }
    }
}