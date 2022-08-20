using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Artemis.Plugins.PhilipsHue.Views.Steps;

public class SyncStep : UserControl
{
    public SyncStep()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}