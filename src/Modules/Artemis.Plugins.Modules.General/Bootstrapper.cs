using Artemis.Core;
using Artemis.Plugins.Modules.General.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Modules.General
{
    public class Bootstrapper : IPluginBootstrapper
    {
        public void Enable(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<GeneralModuleConfigurationViewModel>();
        }

        public void Disable(Plugin plugin)
        {
        }
    }
}