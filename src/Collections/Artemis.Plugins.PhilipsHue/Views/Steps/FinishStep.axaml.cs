using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Artemis.Plugins.PhilipsHue.Views.Steps;

public class FinishStep : UserControl
{
    public FinishStep()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}