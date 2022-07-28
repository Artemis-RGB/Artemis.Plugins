using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Artemis.Plugins.Modules.TestData.Views;

public class TestPluginConfigurationView : UserControl
{
    public TestPluginConfigurationView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}