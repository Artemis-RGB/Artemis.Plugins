using Artemis.Core;
using Artemis.Plugins.Devices.WS281X.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Devices.WS281X
{
    // ReSharper disable once InconsistentNaming
    public class WS281XBootstrapper : PluginBootstrapper
    {
        public override void OnPluginLoaded(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<WS281XConfigurationViewModel>();
        }
    }
}