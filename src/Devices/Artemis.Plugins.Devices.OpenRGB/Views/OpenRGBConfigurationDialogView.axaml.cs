using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Artemis.Plugins.Devices.OpenRGB.Views
{
    public partial class OpenRGBConfigurationDialogView : UserControl
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
