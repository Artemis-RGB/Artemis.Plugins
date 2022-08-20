using Artemis.UI.Shared;

namespace Artemis.Plugins.Devices.DMX.ViewModels.Dialogs
{
    public class AddLedsDialogViewModel : ContentDialogViewModelBase
    {
        private int _amount;

        public AddLedsDialogViewModel()
        {
            _amount = 1;
        }

        public int Amount
        {
            get => _amount;
            set => RaiseAndSetIfChanged(ref _amount, value);
        }
    }
}