using System;
using System.Reactive;
using Artemis.Plugins.LayerBrushes.Particle.Models;
using Artemis.UI.Shared;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Particle.ViewModels.Dialogs
{
    public class ParticleDialogViewModel : ContentDialogViewModelBase
    {
        private readonly ParticleConfiguration _particleConfiguration;
        private readonly ParticleViewModel _particleViewModel;
        private readonly ObservableAsPropertyHelper<bool> _isCustomPath;

        private float _maxHeight;
        private float _maxX;
        private float _maxY;
        private float _maxZ;
        private float _maxWidth;
        private float _minHeight;
        private float _minX;
        private float _minY;
        private float _minZ;
        private float _minWidth;

        private ParticleType _particleType;
        private string _path;


        public ParticleDialogViewModel(ParticleViewModel particleViewModel)
        {
            _particleViewModel = particleViewModel;
            _particleConfiguration = particleViewModel.ParticleConfiguration;

            ParticleType = _particleConfiguration.ParticleType;
            MinWidth = _particleConfiguration.MinWidth;
            MaxWidth = _particleConfiguration.MaxWidth;
            MinHeight = _particleConfiguration.MinHeight;
            MaxHeight = _particleConfiguration.MaxHeight;

            MinX = _particleConfiguration.MinRotationVelocityX;
            MaxX = _particleConfiguration.MaxRotationVelocityX;
            MinY = _particleConfiguration.MinRotationVelocityY;
            MaxY = _particleConfiguration.MaxRotationVelocityY;
            MinZ = _particleConfiguration.MinRotationVelocityZ;
            MaxZ = _particleConfiguration.MaxRotationVelocityZ;

            Path = _particleConfiguration.Path;
            Save = ReactiveCommand.Create(ExecuteSave, ValidationContext.Valid);

            _isCustomPath = this.WhenAnyValue(vm => vm.ParticleType, type => type == ParticleType.Path).ToProperty(this, vm => vm.IsCustomPath);
            this.ValidationRule(
                vm => vm.Path,
                this.WhenAnyValue(vm => vm.ParticleType, vm => vm.Path, (type, path) => type != ParticleType.Path || IsPathValid(path)),
                "Path must contain valid SVG path data"
            );
            
            // Leaving this to NumberBox causes some issues with the value resetting
            this.ValidationRule(vm => vm.MinWidth, this.WhenAnyValue(vm => vm.MinWidth, vm => vm.MaxWidth, (min, max) => min <= max), "Must be less than or equal to max width");
            this.ValidationRule(vm => vm.MaxWidth, this.WhenAnyValue(vm => vm.MinWidth, vm => vm.MaxWidth, (min, max) => max >= min), "Must be greater than or equal to min width");
            this.ValidationRule(vm => vm.MinHeight, this.WhenAnyValue(vm => vm.MinHeight, vm => vm.MaxHeight, (min, max) => min <= max), "Must be less than or equal to max height");
            this.ValidationRule(vm => vm.MaxHeight, this.WhenAnyValue(vm => vm.MinHeight, vm => vm.MaxHeight, (min, max) => max >= min), "Must be greater than or equal to min height");
            
            this.ValidationRule(vm => vm.MinX, this.WhenAnyValue(vm => vm.MinX, vm => vm.MaxX, (min, max) => min <= max), "Must be less than or equal to max X rotation");
            this.ValidationRule(vm => vm.MaxX, this.WhenAnyValue(vm => vm.MinX, vm => vm.MaxX, (min, max) => max >= min), "Must be greater than or equal to min X rotation");
            this.ValidationRule(vm => vm.MinY, this.WhenAnyValue(vm => vm.MinY, vm => vm.MaxY, (min, max) => min <= max), "Must be less than or equal to max Y rotation");
            this.ValidationRule(vm => vm.MaxY, this.WhenAnyValue(vm => vm.MinY, vm => vm.MaxY, (min, max) => max >= min), "Must be greater than or equal to min Y rotation");
            this.ValidationRule(vm => vm.MinZ, this.WhenAnyValue(vm => vm.MinZ, vm => vm.MaxZ, (min, max) => min <= max), "Must be less than or equal to max Z rotation");
            this.ValidationRule(vm => vm.MaxZ, this.WhenAnyValue(vm => vm.MinZ, vm => vm.MaxZ, (min, max) => max >= min), "Must be greater than or equal to min Z rotation");
        }

        public ReactiveCommand<Unit, Unit> Save { get; }

        public ParticleType ParticleType
        {
            get => _particleType;
            set => RaiseAndSetIfChanged(ref _particleType, value);
        }

        public float MinWidth
        {
            get => _minWidth;
            set => RaiseAndSetIfChanged(ref _minWidth, value);
        }

        public float MaxWidth
        {
            get => _maxWidth;
            set => RaiseAndSetIfChanged(ref _maxWidth, value);
        }

        public float MinHeight
        {
            get => _minHeight;
            set => RaiseAndSetIfChanged(ref _minHeight, value);
        }

        public float MaxHeight
        {
            get => _maxHeight;
            set => RaiseAndSetIfChanged(ref _maxHeight, value);
        }

        public float MinX
        {
            get => _minX;
            set => RaiseAndSetIfChanged(ref _minX, value);
        }

        public float MaxX
        {
            get => _maxX;
            set => RaiseAndSetIfChanged(ref _maxX, value);
        }

        public float MinY
        {
            get => _minY;
            set => RaiseAndSetIfChanged(ref _minY, value);
        }

        public float MaxY
        {
            get => _maxY;
            set => RaiseAndSetIfChanged(ref _maxY, value);
        }

        public float MinZ
        {
            get => _minZ;
            set => RaiseAndSetIfChanged(ref _minZ, value);
        }

        public float MaxZ
        {
            get => _maxZ;
            set => RaiseAndSetIfChanged(ref _maxZ, value);
        }

        public string Path
        {
            get => _path;
            set
            {
                string path = value;
                if (value != null && (value.Contains("\'") || value.Contains("\"")))
                    path = value.Replace("\'", "").Replace("\"", "");
                RaiseAndSetIfChanged(ref _path, path);
            }
        }

        public bool IsCustomPath => _isCustomPath.Value;

        private void ExecuteSave()
        {
            if (HasErrors)
                return;

            _particleConfiguration.ParticleType = ParticleType;
            _particleConfiguration.MinWidth = MinWidth;
            _particleConfiguration.MaxWidth = MaxWidth;
            _particleConfiguration.MinHeight = MinHeight;
            _particleConfiguration.MaxHeight = MaxHeight;

            _particleConfiguration.MinRotationVelocityX = MinX;
            _particleConfiguration.MaxRotationVelocityX = MaxX;
            _particleConfiguration.MinRotationVelocityY = MinY;
            _particleConfiguration.MaxRotationVelocityY = MaxY;
            _particleConfiguration.MinRotationVelocityZ = MinZ;
            _particleConfiguration.MaxRotationVelocityZ = MaxZ;

            _particleConfiguration.Path = Path;
            _particleViewModel.Update();
        }

        private bool IsPathValid(string path)
        {
            try
            {
                return SKPath.ParseSvgPathData(path) != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}