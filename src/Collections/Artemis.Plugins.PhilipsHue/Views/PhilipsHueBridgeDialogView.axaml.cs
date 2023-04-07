using System.Threading.Tasks;
using Artemis.Plugins.PhilipsHue.ViewModels;
using Artemis.UI.Shared.Extensions;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using ReactiveUI;

namespace Artemis.Plugins.PhilipsHue.Views;

public partial class PhilipsHueBridgeDialogView : ReactiveUserControl<PhilipsHueBridgeDialogViewModel>
{
    public PhilipsHueBridgeDialogView()
    {
        InitializeComponent();
        this.WhenActivated(_ =>
        {
            this.ClearAllDataValidationErrors();
            Dispatcher.UIThread.Post(DelayedAutoFocus);
        });
    }

    private async void DelayedAutoFocus()
    {
        // Don't ask
        await Task.Delay(200);
        IpInputTextBox.Focus();
    }


    private void IpInput_OnKeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            ViewModel?.Accept();
    }
}