using System.Reactive.Disposables;
using Artemis.Core;
using Artemis.UI.Shared;
using ReactiveUI;

namespace Artemis.Plugins.Devices.Razer.ViewModels
{
    public class RazerConfigurationViewModel : PluginConfigurationViewModel
    {
        public RazerConfigurationViewModel(Plugin plugin, PluginSettings settings) : base(plugin)
        {
            LoadEmulatorDevices = settings.GetSetting("LoadEmulatorDevices", false);
            this.WhenActivated(d =>
            {
                LoadEmulatorDevices.AutoSave = true;
                Disposable.Create(() => LoadEmulatorDevices.AutoSave = false).DisposeWith(d);
            });
        }

        public PluginSetting<bool> LoadEmulatorDevices { get; }
    }
}