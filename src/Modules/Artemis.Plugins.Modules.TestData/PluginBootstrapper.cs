using Artemis.Core;
using Artemis.Plugins.Modules.TestData.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Modules.TestData;

public class Bootstrapper : PluginBootstrapper
{
    public override void OnPluginLoaded(Plugin plugin)
    {
        plugin.ConfigurationDialog = new PluginConfigurationDialog<TestPluginConfigurationViewModel>();
    }
}