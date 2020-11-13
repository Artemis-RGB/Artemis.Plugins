using Artemis.Core;
using Artemis.Plugins.Devices.DMX.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Devices.DMX
{
    public class PluginBootstrapper : IPluginBootstrapper
    {
        public void Enable(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<DMXConfigurationViewModel>();
        }

        public void Disable(Plugin plugin)
        {
        }
    }
}