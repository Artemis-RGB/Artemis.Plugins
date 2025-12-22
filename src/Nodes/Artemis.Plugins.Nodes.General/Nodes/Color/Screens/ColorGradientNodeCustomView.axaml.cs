using Artemis.UI.Shared.Controls.GradientPicker;
using ReactiveUI.Avalonia;

namespace Artemis.Plugins.Nodes.General.Nodes.Color.Screens;

public partial class ColorGradientNodeCustomView : ReactiveUserControl<ColorGradientNodeCustomViewModel>
{
    public ColorGradientNodeCustomView()
    {
        InitializeComponent();
    }


    private void GradientPickerButton_OnFlyoutOpened(GradientPickerButton sender, EventArgs args)
    {
    }

    private void GradientPickerButton_OnFlyoutClosed(GradientPickerButton sender, EventArgs args)
    {
        ViewModel?.StoreGradient();
    }
}