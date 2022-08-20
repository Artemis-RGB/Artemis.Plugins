using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Plugins.Devices.Debug.Settings;
using Artemis.UI.Shared;
using Artemis.UI.Shared.Services;

namespace Artemis.Plugins.Devices.Debug.ViewModels
{
    public class DebugConfigurationViewModel : PluginConfigurationViewModel
    {
        private readonly IWindowService _windowService;
        private readonly PluginSetting<List<DeviceDefinition>> _definitions;

        public DebugConfigurationViewModel(Plugin plugin, PluginSettings settings, IWindowService windowService) : base(plugin)
        {
            _windowService = windowService;
            _definitions = settings.GetSetting("DeviceDefinitions", new List<DeviceDefinition>());
            Definitions = new ObservableCollection<DeviceDefinition>(_definitions.Value);
        }

        public ObservableCollection<DeviceDefinition> Definitions { get; }

        public void SaveChanges()
        {
            // Ignore empty definitions
            _definitions.Value.Clear();
            _definitions.Value.AddRange(Definitions.Where(d => !string.IsNullOrWhiteSpace(d.Layout) || !string.IsNullOrWhiteSpace(d.ImageLayout)));
            _definitions.Save();

            Plugin.GetFeature<DebugDeviceProvider>()?.PopulateDevices();
            Close();
        }

        public void AddDefinition()
        {
            Definitions.Add(new DeviceDefinition());
        }

        public async Task Cancel()
        {
            if (!await _windowService.ShowConfirmContentDialog("Discard changes", "Do you want to discard any changes you made?"))
                return;

            _definitions.RejectChanges();
            Close();
        }
    }
}