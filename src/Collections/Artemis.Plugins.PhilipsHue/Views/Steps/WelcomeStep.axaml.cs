using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Artemis.Plugins.PhilipsHue.Views.Steps;

public class WelcomeStep : UserControl
{
    public WelcomeStep()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}