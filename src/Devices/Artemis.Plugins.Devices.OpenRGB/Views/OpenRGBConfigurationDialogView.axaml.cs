using Artemis.Plugins.Devices.OpenRGB.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

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
