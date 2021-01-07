using Artemis.Core;
using Artemis.UI.Shared;
using RGB.NET.Devices.OpenRGB;
using Stylet;
using System.Collections.Generic;
using System.Linq;

namespace Artemis.Plugins.Devices.OpenRGB
{
    public class OpenRGBConfigurationDialogViewModel : PluginConfigurationViewModel
    {
        private readonly PluginSetting<List<OpenRGBServerDefinition>> _definitions;

        public BindableCollection<OpenRGBServerDefinition> Definitions { get; }

        public OpenRGBConfigurationDialogViewModel(Plugin plugin, PluginSettings settings) : base(plugin)
        {
            _definitions = settings.GetSetting("DeviceDefinitions", new List<OpenRGBServerDefinition>());
            Definitions = new BindableCollection<OpenRGBServerDefinition>(_definitions.Value);
        }

        public void SaveChanges()
        {
            // Ignore empty definitions
            _definitions.Value.Clear();
            _definitions.Value.AddRange(Definitions.Where(d => !string.IsNullOrWhiteSpace(d.Ip) || !string.IsNullOrWhiteSpace(d.ClientName) || d.Port != 0));
            _definitions.Save();

            RequestClose();
        }

        public void Cancel()
        {
            RequestClose();
        }

        public void DeleteRow(object def)
        {
            if (def is OpenRGBServerDefinition serverDefinition)
            {
                Definitions.Remove(serverDefinition);
            }
        }
    }
}