using Artemis.Plugins.Devices.DMX.ViewModels.Dialogs;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace Artemis.Plugins.Devices.DMX.Views.Dialogs;

public partial class AddLedsDialogView : ReactiveUserControl<AddLedsDialogViewModel>
{
    public AddLedsDialogView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}