using System;
using System.Reactive.Disposables;
using Artemis.UI.Shared.Extensions;
using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FluentAvalonia.UI.Controls;
using ReactiveUI;

namespace Artemis.Plugins.LayerBrushes.Ambilight.Screens;

public partial class CapturePropertiesView : ReactiveUserControl<CapturePropertiesViewModel>
{
    public CapturePropertiesView()
    {
        InitializeComponent();
        this.WhenActivated(d => ViewModel.WhenAnyValue(vm => vm.EnableValidation).Subscribe(EnableValidation).DisposeWith(d));
    }

    private void EnableValidation(bool enable)
    {
        foreach (NumberBox numberBox in this.GetVisualChildrenOfType<NumberBox>())
            numberBox.ValidationMode = enable ? NumberBoxValidationMode.InvalidInputOverwritten : NumberBoxValidationMode.Disabled;
    }

    private void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is not IDataContextProvider dataContextProvider || dataContextProvider.DataContext is not CaptureScreenViewModel viewModel)
            return;

        if (ViewModel != null)
            ViewModel.SelectedCaptureScreen = viewModel;
    }

    private void InputFinished(object? sender, RoutedEventArgs e)
    {
        ViewModel?.Save();
    }

    private void PointerInputFinished(object? sender, PointerCaptureLostEventArgs pointerCaptureLostEventArgs)
    {
        ViewModel?.Save();
    }
}