namespace Artemis.Plugins.LayerBrushes.Particle.Models
{
    public class ParticleConfiguration
    {
        public ParticleType ParticleType { get; set; }
        public float SpawnChance { get; set; }

        public float MinWidth { get; set; }
        public float MaxWidth { get; set; }
        public float MinHeight { get; set; }
        public float MaxHeight { get; set; }
        public float[] Points { get; set; }
    }

    public enum ParticleType
    {
        Rectangle,
        Ellipse,
        Path
    }
}