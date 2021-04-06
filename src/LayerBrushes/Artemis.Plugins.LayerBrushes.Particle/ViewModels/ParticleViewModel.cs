using System;
using System.Windows.Media;
using Artemis.Plugins.LayerBrushes.Particle.Models;
using Stylet;

namespace Artemis.Plugins.LayerBrushes.Particle.ViewModels
{
    public class ParticleViewModel : Screen
    {
        private readonly Random _rand;
        private ParticleType _particleType;
        private Geometry _pathGeometry;
        private double _previewHeight;
        private double _previewWidth;

        public ParticleViewModel(ParticleConfiguration particleConfiguration)
        {
            _rand = new Random();

            ParticleConfiguration = particleConfiguration;
            Update();
        }

        public ParticleConfiguration ParticleConfiguration { get; }

        public ParticleType ParticleType
        {
            get => _particleType;
            set => SetAndNotify(ref _particleType, value);
        }

        public double PreviewWidth
        {
            get => _previewWidth;
            set => SetAndNotify(ref _previewWidth, value);
        }

        public double PreviewHeight
        {
            get => _previewHeight;
            set => SetAndNotify(ref _previewHeight, value);
        }

        public Geometry PathGeometry
        {
            get => _pathGeometry;
            set => SetAndNotify(ref _pathGeometry, value);
        }

        public bool IsRectangleType => ParticleType == ParticleType.Rectangle;
        public bool IsEllipseType => ParticleType == ParticleType.Ellipse;
        public bool IsPathType => ParticleType == ParticleType.Path;

        public void Update()
        {
            ParticleType = ParticleConfiguration.ParticleType;
            if (IsPathType)
                PathGeometry = Geometry.Parse(ParticleConfiguration.Path);

            NotifyOfPropertyChange(nameof(ParticleConfiguration));
            NotifyOfPropertyChange(nameof(IsRectangleType));
            NotifyOfPropertyChange(nameof(IsEllipseType));
            NotifyOfPropertyChange(nameof(IsPathType));

            RandomizePreview();
        }

        public void RandomizePreview()
        {
            PreviewWidth = _rand.Next((int) (ParticleConfiguration.MinWidth * 100), (int) (ParticleConfiguration.MaxWidth * 100)) / 100.0;
            PreviewHeight = _rand.Next((int) (ParticleConfiguration.MinHeight * 100), (int) (ParticleConfiguration.MaxHeight * 100)) / 100.0;
        }
    }
}