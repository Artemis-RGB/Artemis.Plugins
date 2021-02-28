using Artemis.Core;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Devices.Razer.ViewModels
{
    public class RazerConfigurationViewModel : PluginConfigurationViewModel
    {
        public RazerConfigurationViewModel(Plugin plugin, PluginSettings settings) : base(plugin)
        {
            LoadEmulatorDevices = settings.GetSetting("LoadEmulatorDevices", false);
        }

        public PluginSetting<bool> LoadEmulatorDevices { get; }

        protected override void OnInitialActivate()
        {
            LoadEmulatorDevices.AutoSave = true;
            base.OnInitialActivate();
        }

        protected override void OnClose()
        {
            LoadEmulatorDevices.AutoSave = false;
            base.OnClose();
        }
    }
}