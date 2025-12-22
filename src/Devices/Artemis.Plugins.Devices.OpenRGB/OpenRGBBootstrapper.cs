using Artemis.Core;
using Artemis.Plugins.Devices.OpenRGB.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Devices.OpenRGB
{
    public class OpenRGBBootstrapper : PluginBootstrapper
    {
        public override void OnPluginLoaded(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<OpenRGBConfigurationDialogViewModel>(true);
        }
    }
}
