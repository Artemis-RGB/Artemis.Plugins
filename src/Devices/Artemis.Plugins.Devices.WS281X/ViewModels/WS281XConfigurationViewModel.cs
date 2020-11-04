using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Artemis.Core;
using Artemis.Plugins.Devices.WS281X.Settings;
using Artemis.Plugins.Devices.WS281X.ViewModels.Dialogs;
using Artemis.UI.Shared.Services;
using Stylet;

namespace Artemis.Plugins.Devices.WS281X.ViewModels
{
    public class WS281XConfigurationViewModel : PluginConfigurationViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly PluginSetting<List<DeviceDefinition>> _definitions;

        public WS281XConfigurationViewModel(Plugin plugin, PluginSettings settings, IDialogService dialogService) : base(plugin)
        {
            _dialogService = dialogService;
            _definitions = settings.GetSetting<List<DeviceDefinition>>("DeviceDefinitions");
            
            Definitions = new BindableCollection<DeviceDefinition>(_definitions.Value);
        }

        public BindableCollection<DeviceDefinition> Definitions { get; }

        public void OpenHyperlink(object sender, RequestNavigateEventArgs e)
        {
            Utilities.OpenUrl(e.Uri.AbsoluteUri);
        }

        public async Task AddDevice()
        {
            DeviceDefinition device = new DeviceDefinition {Name = $"Device {_definitions.Value.Count + 1}"};
            Dictionary<string, object> parameters = new Dictionary<string, object> {{"device", device}};
            await _dialogService.ShowDialogAt<DeviceConfigurationDialogViewModel>("PluginSettingsDialog", parameters);

            _definitions.Value.Add(device);
            Definitions.Add(device);

            _definitions.Save();
        }

        public async Task EditDevice(DeviceDefinition device)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object> {{"device", device}};
            await _dialogService.ShowDialogAt<DeviceConfigurationDialogViewModel>("PluginSettingsDialog", parameters);

            _definitions.Save();
        }

        public void RemoveDevice(DeviceDefinition device)
        {
            _definitions.Value.Remove(device);
            Definitions.Remove(device);

            _definitions.Save();
        }
    }
}