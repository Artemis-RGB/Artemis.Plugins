using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Artemis.Plugins.PhilipsHue.Views.Steps;

public class SettingsStep : UserControl
{
    public SettingsStep()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}