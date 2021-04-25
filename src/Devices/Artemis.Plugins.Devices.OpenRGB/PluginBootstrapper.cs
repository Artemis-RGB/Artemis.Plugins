using Artemis.Core;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Devices.OpenRGB
{
    public class PluginBootstrapper : IPluginBootstrapper
    {
        public void OnPluginLoaded(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<OpenRGBConfigurationDialogViewModel>();
        }

        public void OnPluginEnabled(Plugin plugin)
        {
        }

        public void OnPluginDisabled(Plugin plugin)
        {
        }
    }
}
