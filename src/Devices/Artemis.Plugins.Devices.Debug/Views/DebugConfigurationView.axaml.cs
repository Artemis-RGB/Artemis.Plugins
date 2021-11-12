using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Artemis.Plugins.Devices.Debug.Views
{
    public partial class DebugConfigurationView : UserControl
    {
        public DebugConfigurationView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
