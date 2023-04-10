using Artemis.Plugins.LayerEffects.Filter.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace Artemis.Plugins.LayerEffects.Filter.Views;

public partial class ColorMatrixConfigurationView : ReactiveUserControl<ColorMatrixConfigurationViewModel>
{
    public ColorMatrixConfigurationView()
    {
        InitializeComponent();
    }

}