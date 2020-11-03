using System.Collections.Generic;
using System.Windows.Navigation;
using Artemis.Core;
using Artemis.Plugins.Devices.WS281X.Settings;
using Artemis.Plugins.Devices.WS281X.ViewModels.Dialogs;
using Artemis.UI.Shared.Services;

namespace Artemis.Plugins.Devices.WS281X.ViewModels
{
    public class WS281XConfigurationViewModel : PluginConfigurationViewModel
    {
        private readonly IDialogService _dialogService;
        private PluginSetting<List<DeviceDefinition>> _definitions;

        public WS281XConfigurationViewModel(Plugin plugin, PluginSettings settings, IDialogService dialogService) : base(plugin)
        {
            _dialogService = dialogService;
            _definitions = settings.GetSetting<List<DeviceDefinition>>("DeviceDefinitions");
        }

        public void OpenHyperlink(object sender, RequestNavigateEventArgs e)
        {
            Utilities.OpenUrl(e.Uri.AbsoluteUri);
        }

        public void AddDevice()
        {
            _dialogService.ShowDialog<DeviceConfigurationDialog>();
        }

        public void EditDevice(DeviceDefinition device)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object> {{"device", device}};
        }
    }
}