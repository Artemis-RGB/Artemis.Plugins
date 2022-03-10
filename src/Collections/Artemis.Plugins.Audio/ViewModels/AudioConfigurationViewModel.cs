using Artemis.Core;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Audio.ViewModels
{
    public class AudioConfigurationViewModel : PluginConfigurationViewModel
    {
        public AudioConfigurationViewModel(Plugin plugin, PluginSettings pluginSettings) : base(plugin)
        {
            UseCustomWasapiCapture = pluginSettings.GetSetting("UseCustomWasapiCapture", false);
            UseCustomWasapiCapture.AutoSave = true;
        }

        public override void OnCloseRequested()
        {
            UseCustomWasapiCapture.AutoSave = false;
        }

        public PluginSetting<bool> UseCustomWasapiCapture { get; }
    }
}