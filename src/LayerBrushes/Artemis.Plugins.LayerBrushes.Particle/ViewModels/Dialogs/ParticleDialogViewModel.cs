using System;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Artemis.Core;
using Artemis.Plugins.LayerBrushes.Particle.Models;
using Artemis.UI.Shared;
using Artemis.UI.Shared.Services;
using FluentValidation;
using FluentValidation.Validators;
using SkiaSharp;
using Stylet;

namespace Artemis.Plugins.LayerBrushes.Particle.ViewModels.Dialogs
{
    public class ParticleDialogViewModel : DialogViewModelBase
    {
        private readonly ParticleConfiguration _particleConfiguration;
        private readonly ParticleViewModel _particleViewModel;
        private float _maxHeight;
        private float _maxRotationVelocityX;
        private float _maxRotationVelocityY;
        private float _maxRotationVelocityZ;
        private float _maxWidth;
        private float _minHeight;
        private float _minRotationVelocityX;
        private float _minRotationVelocityY;
        private float _minRotationVelocityZ;
        private float _minWidth;

        private ParticleType _particleType;
        private string _path;

        public ParticleDialogViewModel(ParticleViewModel particleViewModel, IModelValidator<ParticleDialogViewModel> validator) : base(validator)
        {
            _particleViewModel = particleViewModel;
            _particleConfiguration = particleViewModel.ParticleConfiguration;

            ParticleType = _particleConfiguration.ParticleType;
            MinWidth = _particleConfiguration.MinWidth;
            MaxWidth = _particleConfiguration.MaxWidth;
            MinHeight = _particleConfiguration.MinHeight;
            MaxHeight = _particleConfiguration.MaxHeight;

            MinRotationVelocityX = _particleConfiguration.MinRotationVelocityX;
            MaxRotationVelocityX = _particleConfiguration.MaxRotationVelocityX;
            MinRotationVelocityY = _particleConfiguration.MinRotationVelocityY;
            MaxRotationVelocityY = _particleConfiguration.MaxRotationVelocityY;
            MinRotationVelocityZ = _particleConfiguration.MinRotationVelocityZ;
            MaxRotationVelocityZ = _particleConfiguration.MaxRotationVelocityZ;

            Path = _particleConfiguration.Path;

            ParticleTypes = new BindableCollection<ValueDescription>(EnumUtilities.GetAllValuesAndDescriptions(typeof(ParticleType)));
        }

        public ParticleType ParticleType
        {
            get => _particleType;
            set
            {
                SetAndNotify(ref _particleType, value);
                NotifyOfPropertyChange(nameof(IsCustomPath));
            }
        }

        public float MinWidth
        {
            get => _minWidth;
            set
            {
                SetAndNotify(ref _minWidth, value);
                NotifyOfPropertyChange(nameof(MaxWidth));
            }
        }

        public float MaxWidth
        {
            get => _maxWidth;
            set
            {
                SetAndNotify(ref _maxWidth, value);
                NotifyOfPropertyChange(nameof(MinWidth));
            }
        }

        public float MinHeight
        {
            get => _minHeight;
            set
            {
                SetAndNotify(ref _minHeight, value);
                NotifyOfPropertyChange(nameof(MaxHeight));
            }
        }

        public float MaxHeight
        {
            get => _maxHeight;
            set
            {
                SetAndNotify(ref _maxHeight, value);
                NotifyOfPropertyChange(nameof(MinHeight));
            }
        }

        public float MinRotationVelocityX
        {
            get => _minRotationVelocityX;
            set => SetAndNotify(ref _minRotationVelocityX, value);
        }

        public float MaxRotationVelocityX
        {
            get => _maxRotationVelocityX;
            set => SetAndNotify(ref _maxRotationVelocityX, value);
        }

        public float MinRotationVelocityY
        {
            get => _minRotationVelocityY;
            set => SetAndNotify(ref _minRotationVelocityY, value);
        }

        public float MaxRotationVelocityY
        {
            get => _maxRotationVelocityY;
            set => SetAndNotify(ref _maxRotationVelocityY, value);
        }

        public float MinRotationVelocityZ
        {
            get => _minRotationVelocityZ;
            set => SetAndNotify(ref _minRotationVelocityZ, value);
        }

        public float MaxRotationVelocityZ
        {
            get => _maxRotationVelocityZ;
            set => SetAndNotify(ref _maxRotationVelocityZ, value);
        }

        public string Path
        {
            get => _path;
            set
            {
                SetAndNotify(ref _path, value);
                if (Path != null && (Path.Contains("\'") || Path.Contains("\"")))
                    Path = Path.Replace("\'", "").Replace("\"", "");
            }
        }

        public bool IsCustomPath => ParticleType == ParticleType.Path;

        public BindableCollection<ValueDescription> ParticleTypes { get; }

        public async Task Accept()
        {
            await ValidateAsync();

            if (HasErrors)
                return;

            _particleConfiguration.ParticleType = ParticleType;
            _particleConfiguration.MinWidth = MinWidth;
            _particleConfiguration.MaxWidth = MaxWidth;
            _particleConfiguration.MinHeight = MinHeight;
            _particleConfiguration.MaxHeight = MaxHeight;

            _particleConfiguration.MinRotationVelocityX = MinRotationVelocityX;
            _particleConfiguration.MaxRotationVelocityX = MaxRotationVelocityX;
            _particleConfiguration.MinRotationVelocityY = MinRotationVelocityY;
            _particleConfiguration.MaxRotationVelocityY = MaxRotationVelocityY;
            _particleConfiguration.MinRotationVelocityZ = MinRotationVelocityZ;
            _particleConfiguration.MaxRotationVelocityZ = MaxRotationVelocityZ;

            _particleConfiguration.Path = Path;

            _particleViewModel.Update();

            Session.Close(true);
        }

        public void OpenHyperlink(object sender, RequestNavigateEventArgs e)
        {
            Utilities.OpenUrl(e.Uri.AbsoluteUri);
        }
    }

    public class ParticleDialogViewModelValidator : AbstractValidator<ParticleDialogViewModel>
    {
        public ParticleDialogViewModelValidator()
        {
            RuleFor(m => m.ParticleType).NotNull().WithMessage("Particle type is required");
            RuleFor(m => m.MinHeight).LessThanOrEqualTo(m => m.MaxHeight);
            RuleFor(m => m.MaxHeight).GreaterThanOrEqualTo(m => m.MinHeight);
            RuleFor(m => m.MinWidth).LessThanOrEqualTo(m => m.MaxWidth);
            RuleFor(m => m.MaxWidth).GreaterThanOrEqualTo(m => m.MinWidth);
            When(m => m.IsCustomPath, () => RuleFor(m => m.Path).Custom(ValidatePath));
        }

        private void ValidatePath(string input, ValidationContext<ParticleDialogViewModel> validationContext)
        {
            try
            {
                SKPath path = SKPath.ParseSvgPathData(input);
                if (path == null)
                    validationContext.AddFailure("Path must contain valid SVG path data");
            }
            catch (Exception)
            {
                validationContext.AddFailure("Path must contain valid SVG path data");
            }
        }
    }
}