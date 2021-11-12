using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Artemis.Core;
using Artemis.Plugins.Devices.Debug.Settings;
using Artemis.UI.Avalonia.Shared;


namespace Artemis.Plugins.Devices.Debug.ViewModels
{
    public class DebugConfigurationViewModel : PluginConfigurationViewModel
    {
        private readonly PluginSetting<List<DeviceDefinition>> _definitions;

        public DebugConfigurationViewModel(Plugin plugin, PluginSettings settings) : base(plugin)
        {
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

        public void Cancel()
        {
            _definitions.RejectChanges();
            Close();
        }
    }
}