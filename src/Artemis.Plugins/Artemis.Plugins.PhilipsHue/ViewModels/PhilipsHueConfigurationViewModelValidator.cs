using FluentValidation;

namespace Artemis.Plugins.PhilipsHue.ViewModels
{
    public class PhilipsHueConfigurationViewModelValidator : AbstractValidator<PhilipsHueConfigurationViewModel>
    {
        public PhilipsHueConfigurationViewModelValidator()
        {
            RuleFor(m => m.PollingRate).NotEmpty().WithMessage("A polling rate is required");
            RuleFor(m => m.PollingRate).GreaterThanOrEqualTo(1).WithMessage("Polling rate must be 1 second or higher");
        }
    }
}