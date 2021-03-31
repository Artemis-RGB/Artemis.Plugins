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
        private float _maxWidth;
        private float _minHeight;
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

        private void ValidatePath(string arg1, CustomContext arg2)
        {
            try
            {
                SKPath path = SKPath.ParseSvgPathData(arg1);
                if (path == null)
                    arg2.AddFailure("Path must contain valid SVG path data");
            }
            catch (Exception)
            {
                arg2.AddFailure("Path must contain valid SVG path data");
            }
        }
    }
}