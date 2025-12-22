using Artemis.Plugins.Devices.Razer.ViewModels;
using Avalonia.Markup.Xaml;
using ReactiveUI.Avalonia;

namespace Artemis.Plugins.Devices.Razer.Views
{
    public partial class RazerConfigurationView : ReactiveUserControl<RazerConfigurationViewModel>
    {
        public RazerConfigurationView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}