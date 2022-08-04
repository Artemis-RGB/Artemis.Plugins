using Artemis.UI.Shared;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace Artemis.Plugins.Modules.TestData.Views;

public class TestPluginConfigurationView : ReactiveUserControl<PluginConfigurationViewModel>
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