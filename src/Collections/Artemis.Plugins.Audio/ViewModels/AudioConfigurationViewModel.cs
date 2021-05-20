using Artemis.Core;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Audio.ViewModels
{
    public class AudioConfigurationViewModel : PluginConfigurationViewModel
    {
        public AudioConfigurationViewModel(Plugin plugin, PluginSettings pluginSettings) : base(plugin)
        {
            UseCustomWasapiCapture = pluginSettings.GetSetting("UseCustomWasapiCapture", false);
        }

        public PluginSetting<bool> UseCustomWasapiCapture { get; }

        protected override void OnInitialActivate()
        {
            UseCustomWasapiCapture.AutoSave = true;
            base.OnInitialActivate();
        }

        protected override void OnClose()
        {
            UseCustomWasapiCapture.AutoSave = false;
            base.OnClose();
        }
    }
}