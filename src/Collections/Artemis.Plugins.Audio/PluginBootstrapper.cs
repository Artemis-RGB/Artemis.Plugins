using System;
using Artemis.Core;
using Artemis.Plugins.Audio.Services;
using Artemis.Plugins.Audio.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Audio
{
    public class AudioPluginBootstrapper : PluginBootstrapper
    {
        private bool _loaded = false;
        
        public override void OnPluginLoaded(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<AudioConfigurationViewModel>();
        }

        public override void OnPluginEnabled(Plugin plugin)
        {
            if (_loaded)
                return;
            
            if (OperatingSystem.IsWindows())
            {
                plugin.Register<IAudioEnumerationService,NAudioDeviceEnumerationService>();
            }
            if (OperatingSystem.IsLinux())
            {
                plugin.Register<IAudioEnumerationService,OpenAlDeviceEnumerationService>();
            }

            _loaded = true;
        }
    }
}
