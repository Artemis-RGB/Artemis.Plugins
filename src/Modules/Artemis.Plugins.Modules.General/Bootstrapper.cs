using Artemis.Core;
using Artemis.Plugins.Modules.General.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Modules.General
{
    public class Bootstrapper : PluginBootstrapper
    {
        public new void OnPluginLoaded(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<GeneralModuleConfigurationViewModel>();
        }
    }
}