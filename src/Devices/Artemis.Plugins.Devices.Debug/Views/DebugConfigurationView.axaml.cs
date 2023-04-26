using Artemis.Plugins.Devices.Debug.ViewModels;
using Avalonia.ReactiveUI;

namespace Artemis.Plugins.Devices.Debug.Views
{
    public partial class DebugConfigurationView : ReactiveUserControl<DebugConfigurationViewModel>
    {
        public DebugConfigurationView()
        {
            InitializeComponent();
        }
    }
}
