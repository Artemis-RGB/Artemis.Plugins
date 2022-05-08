using Artemis.UI.Shared;
using FluentAvalonia.UI.Controls;
using ReactiveUI.Validation.Extensions;

namespace Artemis.Plugins.PhilipsHue.ViewModels;

public class PhilipsHueBridgeDialogViewModel : ContentDialogViewModelBase
{
    private string _ipAddress;

    public PhilipsHueBridgeDialogViewModel()
    {
        this.ValidationRule(vm => vm.IpAddress, s => !string.IsNullOrWhiteSpace(s), "IP address or hostname is required");
    }

    public string IpAddress
    {
        get => _ipAddress;
        set => RaiseAndSetIfChanged(ref _ipAddress, value);
    }

    public void Accept()
    {
        ContentDialog?.Hide(ContentDialogResult.Primary);
    }
}