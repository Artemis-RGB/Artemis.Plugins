using Artemis.Core;
using Artemis.Plugins.PhilipsHue.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.PhilipsHue
{
    public class HueBootstrapper : IPluginBootstrapper
    {
        public void Enable(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<PhilipsHueConfigurationViewModel>();
        }

        public void Disable(Plugin plugin)
        {
        }
    }
}