using Artemis.Core;
using Artemis.Plugins.DataModelExpansions.TestData.Prerequisites;
using Artemis.Plugins.DataModelExpansions.TestData.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.DataModelExpansions.TestData
{
    public class MyPluginBootstrapper : PluginBootstrapper
    {
        public override void OnPluginLoaded(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<TestPluginConfigurationViewModel>();
            AddFeaturePrerequisite<PluginDataModelExpansion>(new TestPrerequisite1());
            AddFeaturePrerequisite<PluginDataModelExpansion>(new TestPrerequisite2());
        }
    }
}