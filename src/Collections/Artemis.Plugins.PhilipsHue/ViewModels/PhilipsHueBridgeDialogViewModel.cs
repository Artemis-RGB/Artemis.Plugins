using Artemis.Plugins.PhilipsHue.Models;
using Artemis.UI.Shared.Services;

namespace Artemis.Plugins.PhilipsHue.ViewModels
{
    public class PhilipsHueBridgeDialogViewModel : DialogViewModelBase
    {
        public string BridgeId { get; set; }
        public string Hostname { get; set; }


        public void Accept()
        {
            if (Session != null && !Session.IsEnded)
                Session.Close(new PhilipsHueBridge {BridgeId = BridgeId, IpAddress = Hostname});
        }

        public new void Cancel()
        {
            if (Session != null && !Session.IsEnded)
                Session.Close();
        }
    }
}