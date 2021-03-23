using Artemis.Core;
using Artemis.Plugins.LayerBrushes.Particle.SKParticle;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Particle.PropertyGroups
{
    public class MainPropertyGroup : LayerPropertyGroup
    {
        [PropertyDescription(Description = "How many particles to spawn per second")]
        public IntLayerProperty ParticleRate { get; set; }

        [PropertyDescription(Description = "The angle at which to spawn particles", InputAffix = "°")]
        public FloatLayerProperty Angle { get; set; }

        [PropertyDescription(Description = "How far to spread the particles out", InputAffix = "°", MinInputValue = 0f)]
        public FloatLayerProperty Spread { get; set; }

        [PropertyDescription]
        public FloatRangeLayerProperty InitialVelocity { get; set; }

        [PropertyDescription]
        public FloatRangeLayerProperty RotationVelocity { get; set; }

        [PropertyDescription]
        public FloatLayerProperty MaximumVelocity { get; set; }

        [PropertyDescription]
        public FloatLayerProperty Lifetime { get; set; }

        [PropertyDescription]
        public BoolLayerProperty FadeOut { get; set; }

        [PropertyDescription]
        public SKPointLayerProperty Gravity { get; set; }

        [PropertyDescription]
        public EnumLayerProperty<SKConfettiEmitterSide> EmitterSide { get; set; }

        [PropertyDescription]
        public ColorGradientLayerProperty Colors { get; set; }

        protected override void PopulateDefaults()
        {
            ParticleRate.DefaultValue = 40;
            Angle.DefaultValue = 90f;
            Spread.DefaultValue = 15f;
            InitialVelocity.DefaultValue = new FloatRange(100f, 200f);
            RotationVelocity.DefaultValue = new FloatRange(10f, 75f);
            MaximumVelocity.DefaultValue = 0f;
            Lifetime.DefaultValue = 2f;
            FadeOut.DefaultValue = true;
            Gravity.DefaultValue = new SKPoint(0, 9.81f);

            Colors.DefaultValue = ColorGradient.GetUnicornBarf();
        }

        protected override void EnableProperties()
        {
            // This is where you do any sort of initialization on your property group
        }

        protected override void DisableProperties()
        {
            // If you subscribed to events or need to clean up, do it here
        }
    }
}