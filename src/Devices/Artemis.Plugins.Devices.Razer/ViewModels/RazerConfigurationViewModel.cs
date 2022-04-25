using Artemis.Core;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Devices.Razer.ViewModels
{
    public class RazerConfigurationViewModel : PluginConfigurationViewModel
    {
        public RazerConfigurationViewModel(Plugin plugin, PluginSettings settings) : base(plugin)
        {
            LoadEmulatorDevices = settings.GetSetting("LoadEmulatorDevices", false);
            LoadEmulatorDevices.AutoSave = true;
        }

        public PluginSetting<bool> LoadEmulatorDevices { get; }

        public override void OnCloseRequested()
        {
            LoadEmulatorDevices.AutoSave = false;
        }
    }
}