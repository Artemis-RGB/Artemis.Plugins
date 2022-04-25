using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Artemis.Plugins.Devices.Razer.Views
{
    public partial class RazerConfigurationView : UserControl
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