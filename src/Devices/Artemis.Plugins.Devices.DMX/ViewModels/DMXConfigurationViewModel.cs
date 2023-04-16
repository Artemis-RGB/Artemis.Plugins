using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.Plugins.Devices.DMX.Settings;
using Artemis.Plugins.Devices.DMX.ViewModels.Dialogs;
using Artemis.UI.Shared;
using Artemis.UI.Shared.Services;
using DynamicData;
using ReactiveUI;

namespace Artemis.Plugins.Devices.DMX.ViewModels
{
    public class DMXConfigurationViewModel : PluginConfigurationViewModel
    {
        private readonly PluginSetting<List<DeviceDefinition>> _definitions;
        private readonly IPluginManagementService _pluginManagementService;
        private readonly PluginSettings _settings;
        private readonly IWindowService _windowService;

        public DMXConfigurationViewModel(Plugin plugin, PluginSettings settings, IWindowService windowService, IPluginManagementService pluginManagementService)
            : base(plugin)
        {
            _settings = settings;
            _windowService = windowService;
            _pluginManagementService = pluginManagementService;
            _definitions = _settings.GetSetting("DeviceDefinitions", new List<DeviceDefinition>());

            Definitions = new ObservableCollection<DeviceDefinition>(_definitions.Value);
            TurnOffLedsOnShutdown = _settings.GetSetting("TurnOffLedsOnShutdown", false);

            AddDevice = ReactiveCommand.CreateFromTask(ExecuteAddDevice);
            EditDevice = ReactiveCommand.CreateFromTask<DeviceDefinition>(ExecuteEditDevice);
            RemoveDevice = ReactiveCommand.Create<DeviceDefinition>(ExecuteRemoveDevice);
            Save = ReactiveCommand.Create(ExecuteSave);
            Cancel = ReactiveCommand.CreateFromTask(ExecuteCancel);
        }

        public ReactiveCommand<Unit, Unit> AddDevice { get; }
        public ReactiveCommand<DeviceDefinition, Unit> EditDevice { get; }
        public ReactiveCommand<DeviceDefinition, Unit> RemoveDevice { get; }
        public ReactiveCommand<Unit, Unit> Save { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }

        public ObservableCollection<DeviceDefinition> Definitions { get; }
        public PluginSetting<bool> TurnOffLedsOnShutdown { get; }

        private async Task ExecuteAddDevice()
        {
            DeviceDefinition device = new()
            {
                Name = $"Device {_definitions.Value.Count + 1}",
                Port = 5568,
                Universe = 1
            };
            if (await _windowService.ShowDialogAsync<DeviceConfigurationDialogViewModel, DeviceDialogResult>(device) != DeviceDialogResult.Save)
                return;

            _definitions.Value.Add(device);
            Definitions.Add(device);
        }

        private async Task ExecuteEditDevice(DeviceDefinition device)
        {
            if (await _windowService.ShowDialogAsync<DeviceConfigurationDialogViewModel, DeviceDialogResult>(device) == DeviceDialogResult.Remove)
                Definitions.Remove(device);
        }

        private async Task ExecuteCancel()
        {
            if (TurnOffLedsOnShutdown.HasChanged || _definitions.HasChanged)
            {
                if (!await _windowService.ShowConfirmContentDialog("Discard changes", "Do you want to discard any changes you made?"))
                    return;
            }

            _definitions.RejectChanges();
            _settings.GetSetting("TurnOffLedsOnShutdown", false).RejectChanges();

            Close();
        }

        private void ExecuteRemoveDevice(DeviceDefinition device)
        {
            _definitions.Value.Remove(device);
            Definitions.Remove(device);
        }

        private void ExecuteSave()
        {
            _definitions.Save();
            TurnOffLedsOnShutdown.Save();

            // Fire & forget re-enabling the plugin
            Task.Run(() =>
            {
                DMXDeviceProvider deviceProvider = Plugin.GetFeature<DMXDeviceProvider>();
                if (deviceProvider == null || !deviceProvider.IsEnabled) return;
                _pluginManagementService.DisablePluginFeature(deviceProvider, false);
                _pluginManagementService.EnablePluginFeature(deviceProvider, false);
            });

            Close();
        }
    }
}