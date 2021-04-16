using Artemis.UI.Shared.Services;

namespace Artemis.Plugins.Devices.DMX.ViewModels.Dialogs
{
    public class AddLedsDialogViewModel : DialogViewModelBase
    {
        private int _amount;

        public AddLedsDialogViewModel()
        {
            Amount = 1;
        }

        public int Amount
        {
            get => _amount;
            set => SetAndNotify(ref _amount, value);
        }

        public void Accept()
        {
            Session.Close(Amount);
        }

        public void Cancel()
        {
            Session.Close(0);
        }
    }
}