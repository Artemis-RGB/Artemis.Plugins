using Artemis.Plugins.Devices.OpenRGB.ViewModels;
using Avalonia.Markup.Xaml;
using ReactiveUI.Avalonia;

namespace Artemis.Plugins.Devices.OpenRGB.Views
{
    public partial class OpenRGBConfigurationDialogView : ReactiveUserControl<OpenRGBConfigurationDialogViewModel>
    {
        public OpenRGBConfigurationDialogView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
