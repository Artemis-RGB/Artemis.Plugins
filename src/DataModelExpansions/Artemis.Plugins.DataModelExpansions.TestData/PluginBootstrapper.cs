using Artemis.Core;
using Artemis.Plugins.DataModelExpansions.TestData.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.DataModelExpansions.TestData
{
    public class PluginBootstrapper : IPluginBootstrapper
    {
        public void Enable(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<TestPluginConfigurationViewModel>();
        }

        public void Disable(Plugin plugin)
        {
        }
    }
}