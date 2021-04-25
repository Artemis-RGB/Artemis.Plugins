using Artemis.Core;
using Artemis.Plugins.PhilipsHue.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.PhilipsHue
{
    public class HueBootstrapper : IPluginBootstrapper
    {
        public void OnPluginLoaded(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<PhilipsHueConfigurationViewModel>();
        }

        public void OnPluginEnabled(Plugin plugin)
        {
        }

        public void OnPluginDisabled(Plugin plugin)
        {
        }
    }
}