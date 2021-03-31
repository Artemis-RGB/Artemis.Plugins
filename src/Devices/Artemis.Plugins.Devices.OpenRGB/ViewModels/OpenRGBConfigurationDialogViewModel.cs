using Artemis.Core;
using Artemis.Core.Services;
using Artemis.UI.Shared;
using RGB.NET.Devices.OpenRGB;
using Stylet;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artemis.Plugins.Devices.OpenRGB
{
    public class OpenRGBConfigurationDialogViewModel : PluginConfigurationViewModel
    {
        private readonly IPluginManagementService _pluginManagementService;
        private readonly PluginSetting<bool> _forceAddAllDevicesSetting;
        private readonly PluginSetting<List<OpenRGBServerDefinition>> _definitions;
        private bool _forceAddAllDevices;

        public BindableCollection<OpenRGBServerDefinition> Definitions { get; }
        public bool ForceAddAllDevices
        {
            get => _forceAddAllDevices;
            set => SetAndNotify(ref _forceAddAllDevices, value);
        }

        public OpenRGBConfigurationDialogViewModel(Plugin plugin, PluginSettings settings, IPluginManagementService pluginManagementService) : base(plugin)
        {
            _pluginManagementService = pluginManagementService;
            _definitions = settings.GetSetting("DeviceDefinitions", new List<OpenRGBServerDefinition>());
            _forceAddAllDevicesSetting = settings.GetSetting("ForceAddAllDevices", false);

            Definitions = new BindableCollection<OpenRGBServerDefinition>(_definitions.Value);
            ForceAddAllDevices = _forceAddAllDevicesSetting.Value;
        }

        public void DeleteRow(object def)
        {
            if (def is OpenRGBServerDefinition serverDefinition)
            {
                Definitions.Remove(serverDefinition);
            }
        }

        protected override void OnClose()
        {
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
            base.OnClose();
        }
    }
}