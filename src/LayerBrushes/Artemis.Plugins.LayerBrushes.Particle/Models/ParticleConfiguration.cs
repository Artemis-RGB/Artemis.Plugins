namespace Artemis.Plugins.LayerBrushes.Particle.Models
{
    public class ParticleConfiguration
    {
        public ParticleType ParticleType { get; set; }
        public string Path { get; set; }

        public float MinWidth { get; set; } = 10;
        public float MaxWidth { get; set; } = 20;
        public float MinHeight { get; set; } = 10;
        public float MaxHeight { get; set; } = 20;
    }

    public enum ParticleType
    {
        Rectangle,
        Ellipse,
        Path
    }
}