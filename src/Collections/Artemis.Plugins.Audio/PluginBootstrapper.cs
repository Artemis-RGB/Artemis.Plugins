using Artemis.Core;
using Artemis.Plugins.Audio.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Audio
{
    public class AudioPluginBootstrapper : PluginBootstrapper
    {
        public override void OnPluginLoaded(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<AudioConfigurationViewModel>();
        }
    }
}
