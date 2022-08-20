using System;
using Artemis.Plugins.LayerBrushes.Particle.Models;
using Artemis.UI.Shared;
using Avalonia.Media;
using ReactiveUI;

namespace Artemis.Plugins.LayerBrushes.Particle.ViewModels
{
    public class ParticleViewModel : ViewModelBase
    {
        private readonly Random _rand;
        private ParticleType _particleType;
        private Geometry _pathGeometry;
        private double _previewHeight;
        private double _previewWidth;

        public ParticleViewModel(ParticleConfiguration particleConfiguration)
        {
            _rand = new Random();

            ParticleConfiguration = new ParticleConfiguration(particleConfiguration);
            Update();
        }

        public ParticleConfiguration ParticleConfiguration { get; }

        public ParticleType ParticleType
        {
            get => _particleType;
            set => RaiseAndSetIfChanged(ref _particleType, value);
        }

        public double PreviewWidth
        {
            get => _previewWidth;
            set => RaiseAndSetIfChanged(ref _previewWidth, value);
        }

        public double PreviewHeight
        {
            get => _previewHeight;
            set => RaiseAndSetIfChanged(ref _previewHeight, value);
        }

        public Geometry PathGeometry
        {
            get => _pathGeometry;
            set => RaiseAndSetIfChanged(ref _pathGeometry, value);
        }

        public bool IsRectangleType => ParticleType == ParticleType.Rectangle;
        public bool IsEllipseType => ParticleType == ParticleType.Ellipse;
        public bool IsPathType => ParticleType == ParticleType.Path;

        public void Update()
        {
            ParticleType = ParticleConfiguration.ParticleType;
            if (IsPathType)
                PathGeometry = Geometry.Parse(ParticleConfiguration.Path);

            this.RaisePropertyChanged(nameof(ParticleConfiguration));
            this.RaisePropertyChanged(nameof(IsRectangleType));
            this.RaisePropertyChanged(nameof(IsEllipseType));
            this.RaisePropertyChanged(nameof(IsPathType));

            RandomizePreview();
        }

        public void RandomizePreview()
        {
            PreviewWidth = _rand.Next((int) (ParticleConfiguration.MinWidth * 100), (int) (ParticleConfiguration.MaxWidth * 100)) / 100.0;
            PreviewHeight = _rand.Next((int) (ParticleConfiguration.MinHeight * 100), (int) (ParticleConfiguration.MaxHeight * 100)) / 100.0;
        }
    }
}