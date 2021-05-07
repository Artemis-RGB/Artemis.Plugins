using Artemis.Core;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Audio.SettingsDialog
{
    public class AudioPluginConfigurationViewModel : PluginConfigurationViewModel
    {
        private readonly PluginSetting<bool> _useCustomWasapiCaptureSetting;
        private bool _useCustomWasapiCapture;


        public bool UseCustomWasapiCapture
        {
            get => _useCustomWasapiCapture;
            set => SetAndNotify(ref _useCustomWasapiCapture, value);
        }

        public AudioPluginConfigurationViewModel(Plugin plugin, PluginSettings pluginSettings) : base(plugin)
        {
            _useCustomWasapiCaptureSetting = pluginSettings.GetSetting<bool>("UseCustomWasapiCapture", false);
            _useCustomWasapiCapture = _useCustomWasapiCaptureSetting.Value;
        }

        public void Save()
        {
            _useCustomWasapiCaptureSetting.Value = _useCustomWasapiCapture;
            _useCustomWasapiCaptureSetting.Save();
            RequestClose();
        }

      
        public void Reset()
        {
            _useCustomWasapiCapture = false;
            _useCustomWasapiCaptureSetting.Value = _useCustomWasapiCapture;
            _useCustomWasapiCaptureSetting.Save();
        }

        public void Cancel()
        {
            RequestClose();
        }
    }
}
