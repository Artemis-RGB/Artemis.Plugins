using Artemis.UI.Shared;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace Artemis.Plugins.Modules.TestData.Views;

public partial class TestPluginConfigurationView : ReactiveUserControl<PluginConfigurationViewModel>
{
    public TestPluginConfigurationView()
    {
        InitializeComponent();
    }
}