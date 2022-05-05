using Artemis.Plugins.Devices.Razer.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

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