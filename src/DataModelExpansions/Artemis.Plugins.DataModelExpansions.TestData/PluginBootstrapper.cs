using Artemis.Core;
using Artemis.Plugins.DataModelExpansions.TestData.Prerequisites;
using Artemis.Plugins.DataModelExpansions.TestData.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.DataModelExpansions.TestData
{
    public class PluginBootstrapper : IPluginBootstrapper
    {
        public void OnPluginLoaded(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<TestPluginConfigurationViewModel>();
            plugin.Prerequisites.Add(new TestPrerequisite1(plugin));
            plugin.Prerequisites.Add(new TestPrerequisite2(plugin));
        }

        public void OnPluginEnabled(Plugin plugin)
        {
        }

        public void OnPluginDisabled(Plugin plugin)
        {
        }
    }
}