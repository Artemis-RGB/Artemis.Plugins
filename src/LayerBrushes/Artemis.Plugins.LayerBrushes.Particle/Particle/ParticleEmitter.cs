using System;

namespace Artemis.Plugins.LayerBrushes.Particle.Particle
{
    public class ParticleEmitter
    {
        private double _totalParticles;

        public int ParticleRate { get; set; } = 100;
        public int MaxParticles { get; set; } = -1;
        public bool IsComplete { get; set; }

        public void Update(TimeSpan deltaTime)
        {
            if (IsComplete || deltaTime < TimeSpan.Zero)
                return;

            int before = (int) _totalParticles;

            double particles = ParticleRate * deltaTime.TotalSeconds;
            if (MaxParticles > 0)
                particles = Math.Min(particles, MaxParticles);

            _totalParticles += particles;

            int after = (int) _totalParticles;
            if (before < after) OnParticlesCreated(after - before);


            IsComplete = MaxParticles > 0 && _totalParticles >= MaxParticles;
        }

        public event Action<int> ParticlesCreated;

        protected virtual void OnParticlesCreated(int particles)
        {
            ParticlesCreated?.Invoke(particles);
        }
    }
}