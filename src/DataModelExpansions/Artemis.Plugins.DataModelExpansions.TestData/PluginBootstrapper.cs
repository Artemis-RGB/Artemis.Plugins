using Artemis.Core;
using Artemis.Plugins.DataModelExpansions.TestData.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.DataModelExpansions.TestData
{
    public class Bootstrapper : PluginBootstrapper
    {
        public override void OnPluginLoaded(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<TestPluginConfigurationViewModel>();
        }
    }
}