using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.Plugins.Devices.WS281X.Settings;
using Artemis.Plugins.Devices.WS281X.ViewModels.Dialogs;
using Artemis.UI.Shared;
using Artemis.UI.Shared.Services;
using Stylet;

namespace Artemis.Plugins.Devices.WS281X.ViewModels
{
    public class WS281XConfigurationViewModel : PluginConfigurationViewModel
    {
        private readonly PluginSetting<List<DeviceDefinition>> _definitions;
        private readonly IDialogService _dialogService;
        private readonly IPluginManagementService _pluginManagementService;
        private readonly PluginSettings _settings;
        private bool _turnOffLedsOnShutdown;

        public WS281XConfigurationViewModel(Plugin plugin, PluginSettings settings, IDialogService dialogService, IPluginManagementService pluginManagementService)
            : base(plugin)
        {
            _settings = settings;
            _dialogService = dialogService;
            _pluginManagementService = pluginManagementService;
            _definitions = _settings.GetSetting("DeviceDefinitions", new List<DeviceDefinition>());

            TurnOffLedsOnShutdown = _settings.GetSetting("TurnOffLedsOnShutdown", false).Value;
            Definitions = new BindableCollection<DeviceDefinition>(_definitions.Value);
        }

        public BindableCollection<DeviceDefinition> Definitions { get; }

        public bool TurnOffLedsOnShutdown
        {
            get => _turnOffLedsOnShutdown;
            set => SetAndNotify(ref _turnOffLedsOnShutdown, value);
        }

        public void OpenHyperlink(object sender, RequestNavigateEventArgs e)
        {
            Utilities.OpenUrl(e.Uri.AbsoluteUri);
        }

        public async Task AddDevice()
        {
            DeviceDefinition device = new() {Name = $"Device {_definitions.Value.Count + 1}"};
            Dictionary<string, object> parameters = new() {{"device", device}};
            object result = await _dialogService.ShowDialogAt<DeviceConfigurationDialogViewModel>("PluginSettingsDialog", parameters);

            if (result is bool boolResult && boolResult == false || result is not bool)
                return;

            _definitions.Value.Add(device);
            Definitions.Add(device);

            _definitions.Save();
        }

        public async Task EditDevice(DeviceDefinition device)
        {
            Dictionary<string, object> parameters = new() {{"device", device}};
            await _dialogService.ShowDialogAt<DeviceConfigurationDialogViewModel>("PluginSettingsDialog", parameters);

            _definitions.Save();
        }

        public void RemoveDevice(DeviceDefinition device)
        {
            _definitions.Value.Remove(device);
            Definitions.Remove(device);

            _definitions.Save();
        }

        #region Overrides of Screen

        /// <inheritdoc />
        protected override void OnClose()
        {
            _settings.GetSetting("TurnOffLedsOnShutdown", false).Value = TurnOffLedsOnShutdown;
            _settings.GetSetting("TurnOffLedsOnShutdown", false).Save();

            // Fire & forget re-enabling the plugin
            Task.Run(() =>
            {
                WS281XDeviceProvider deviceProvider = Plugin.GetFeature<WS281XDeviceProvider>();
                if (deviceProvider == null || !deviceProvider.IsEnabled) return;
                _pluginManagementService.DisablePluginFeature(deviceProvider, false);
                _pluginManagementService.EnablePluginFeature(deviceProvider, false);
            });
            
            base.OnClose();
        }

        #endregion
    }
}