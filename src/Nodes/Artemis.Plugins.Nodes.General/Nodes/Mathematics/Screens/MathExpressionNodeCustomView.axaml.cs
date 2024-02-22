using Avalonia.Interactivity;
using Avalonia.ReactiveUI;

namespace Artemis.Plugins.Nodes.General.Nodes.Mathematics.Screens;

public partial class MathExpressionNodeCustomView : ReactiveUserControl<MathExpressionNodeCustomViewModel>
{
    public MathExpressionNodeCustomView()
    {
        InitializeComponent();
    }


    private void InputElement_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        ViewModel?.UpdateInputValue();
    }
}