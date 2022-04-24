using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.UI.Shared;
using Artemis.UI.Shared.Services;
using RGB.NET.Devices.OpenRGB;

namespace Artemis.Plugins.Devices.OpenRGB.ViewModels
{
    public class OpenRGBConfigurationDialogViewModel : PluginConfigurationViewModel
    {
        private readonly PluginSetting<List<OpenRGBServerDefinition>> _definitions;
        private readonly PluginSetting<bool> _forceAddAllDevicesSetting;
        private readonly IPluginManagementService _pluginManagementService;
        private readonly IWindowService _windowService;
        private bool _forceAddAllDevices;

        public OpenRGBConfigurationDialogViewModel(Plugin plugin, PluginSettings settings, IPluginManagementService pluginManagementService, IWindowService windowService) : base(plugin)
        {
            _pluginManagementService = pluginManagementService;
            _windowService = windowService;
            _definitions = settings.GetSetting("DeviceDefinitions", new List<OpenRGBServerDefinition>());
            _forceAddAllDevicesSetting = settings.GetSetting("ForceAddAllDevices", false);

            Definitions = new ObservableCollection<OpenRGBServerDefinition>(_definitions.Value);
            ForceAddAllDevices = _forceAddAllDevicesSetting.Value;
        }

        public ObservableCollection<OpenRGBServerDefinition> Definitions { get; }

        public bool ForceAddAllDevices
        {
            get => _forceAddAllDevices;
            set => this.RaiseAndSetIfChanged(ref _forceAddAllDevices, value);
        }

        public void AddDefinition()
        {
            Definitions.Add(new OpenRGBServerDefinition());
        }

        public void DeleteRow(object def)
        {
            if (def is OpenRGBServerDefinition serverDefinition) 
                Definitions.Remove(serverDefinition);
        }

        public void SaveChanges()
        {
            // Ignore empty definitions
            _definitions.Value.Clear();
            _definitions.Value.AddRange(Definitions.Where(d => !string.IsNullOrWhiteSpace(d.Ip) || !string.IsNullOrWhiteSpace(d.ClientName) || d.Port != 0));
            _definitions.Save();

            _forceAddAllDevicesSetting.Value = ForceAddAllDevices;
            _forceAddAllDevicesSetting.Save();

            // Fire & forget re-enabling the plugin
            Task.Run(() =>
            {
                OpenRGBDeviceProvider deviceProvider = Plugin.GetFeature<OpenRGBDeviceProvider>();
                if (deviceProvider == null || !deviceProvider.IsEnabled) return;
                _pluginManagementService.DisablePluginFeature(deviceProvider, false);
                _pluginManagementService.EnablePluginFeature(deviceProvider, false);
            });
            Close();
        }

        public async Task Cancel()
        {
            if (!await _windowService.ShowConfirmContentDialog("Discard changes", "Do you want to discard any changes you made?"))
                return;

            _definitions.RejectChanges();
            _forceAddAllDevicesSetting.RejectChanges();
            Close();
        }
    }
}