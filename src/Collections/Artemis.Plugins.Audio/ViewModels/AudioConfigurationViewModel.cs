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
            EnableCaptureDeviceAccess = pluginSettings.GetSetting("EnableCaptureDeviceAccess", true);
            EnableCaptureDeviceAccess.AutoSave = true;
        }

        public PluginSetting<bool> UseCustomWasapiCapture { get; }
        public PluginSetting<bool> EnableCaptureDeviceAccess { get; }

        public override void OnCloseRequested()
        {
            UseCustomWasapiCapture.AutoSave = false;
            EnableCaptureDeviceAccess.AutoSave = false;
        }
    }
}