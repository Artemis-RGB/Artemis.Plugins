using System;

namespace Artemis.Plugins.LayerBrushes.Particle.Models
{
    public class ParticleConfiguration
    {
        private static readonly Random Rand = new();

        public ParticleType ParticleType { get; set; }
        public string Path { get; set; }

        public float MinWidth { get; set; } = 10;
        public float MaxWidth { get; set; } = 20;
        public float MinHeight { get; set; } = 10;
        public float MaxHeight { get; set; } = 20;

        public float MinRotationVelocityX { get; set; }
        public float MaxRotationVelocityX { get; set; }
        public float MinRotationVelocityY { get; set; }
        public float MaxRotationVelocityY { get; set; }
        public float MinRotationVelocityZ { get; set; }
        public float MaxRotationVelocityZ { get; set; }

        public float GetWidth()
        {
            return Rand.Next((int) (MinWidth * 100), (int) (MaxWidth * 100)) / 100f;
        }

        public float GetHeight()
        {
            return Rand.Next((int) (MinHeight * 100), (int) (MaxHeight * 100)) / 100f;
        }

        public float GetRotationVelocityX()
        {
            return Rand.Next((int) (MinRotationVelocityX * 100), (int) (MaxRotationVelocityX * 100)) / 100f;
        }

        public float GetRotationVelocityY()
        {
            return Rand.Next((int) (MinRotationVelocityY * 100), (int) (MaxRotationVelocityY * 100)) / 100f;
        }

        public float GetRotationVelocityZ()
        {
            return Rand.Next((int) (MinRotationVelocityZ * 100), (int) (MaxRotationVelocityZ * 100)) / 100f;
        }
    }

    public enum ParticleType
    {
        Rectangle,
        Ellipse,
        Path
    }
}