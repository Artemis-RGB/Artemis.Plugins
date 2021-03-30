using System.Collections.Generic;
using System.Linq;
using Artemis.Plugins.LayerBrushes.Particle.Models;
using Stylet;

namespace Artemis.Plugins.LayerBrushes.Particle.ViewModels
{
    public class ParticleViewModel : Screen
    {
        private readonly ParticleConfiguration _particleConfiguration;
        private float _minHeight;

        private ParticleType _particleType;
        private float _spawnChance;
        private float _minWidth;
        private float _maxWidth;
        private float _maxHeight;

        public ParticleViewModel(ParticleConfiguration particleConfiguration)
        {
            _particleConfiguration = particleConfiguration;

            ParticleType = particleConfiguration.ParticleType;
            SpawnChance = particleConfiguration.SpawnChance;
            MinWidth = particleConfiguration.MinWidth;
            MaxWidth = particleConfiguration.MaxWidth;
            MinHeight = particleConfiguration.MinHeight;
            MaxHeight = particleConfiguration.MaxHeight;
            Points = particleConfiguration.Points.ToList();
        }

        public ParticleType ParticleType
        {
            get => _particleType;
            set => SetAndNotify(ref _particleType, value);
        }

        public float SpawnChance
        {
            get => _spawnChance;
            set => SetAndNotify(ref _spawnChance, value);
        }

        public float MinWidth
        {
            get => _minWidth;
            set => SetAndNotify(ref _minWidth, value);
        }

        public float MaxWidth
        {
            get => _maxWidth;
            set => SetAndNotify(ref _maxWidth, value);
        }

        public float MinHeight
        {
            get => _minHeight;
            set => SetAndNotify(ref _minHeight, value);
        }

        public float MaxHeight
        {
            get => _maxHeight;
            set => SetAndNotify(ref _maxHeight, value);
        }

        public List<float> Points { get; }

        public void Apply()
        {
            _particleConfiguration.ParticleType = ParticleType;
            _particleConfiguration.SpawnChance = SpawnChance;
            _particleConfiguration.MinWidth = MinWidth;
            _particleConfiguration.MaxWidth = MaxWidth;
            _particleConfiguration.MinHeight = MinHeight;
            _particleConfiguration.MaxHeight = MaxHeight;
            _particleConfiguration.Points = Points.ToArray();

            RequestClose();
        }
    }
}