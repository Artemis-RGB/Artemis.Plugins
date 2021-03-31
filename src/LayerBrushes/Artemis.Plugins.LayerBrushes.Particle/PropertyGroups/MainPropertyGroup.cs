using System.Collections.Generic;
using Artemis.Core;
using Artemis.Plugins.LayerBrushes.Particle.Models;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Particle.PropertyGroups
{
    public class MainPropertyGroup : LayerPropertyGroup
    {
        [PropertyDescription(Description = "The emitter is responsible for spawning (emitting) new particles")]
        public EmitterPropertyGroup Emitter { get; set; }

        public ParticlesPropertyGroup Particles { get; set; }

        [PropertyDescription]
        public SKPointLayerProperty Gravity { get; set; }

        [PropertyDescription]
        public LayerProperty<List<ParticleConfiguration>> ParticleConfigurations { get; set; }

        protected override void PopulateDefaults()
        {
            Gravity.DefaultValue = new SKPoint(0, 9.81f);
            ParticleConfigurations.DefaultValue = new List<ParticleConfiguration>();
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

    public class EmitterPropertyGroup : LayerPropertyGroup
    {
        [PropertyDescription(Description = "How many particles to emit per second")]
        public IntLayerProperty ParticleRate { get; set; }

        [PropertyDescription(Description = "The position determines where the particles spawn")]
        public EnumLayerProperty<EmitterPosition> EmitterPosition { get; set; }

        [PropertyDescription(Description = "The X and Y position of the emitter in percentage (50% being the middle of the layer)", InputAffix = "%")]
        public SKPointLayerProperty CustomEmitterPosition { get; set; }

        [PropertyDescription(Description = "The width and height of the emitter in percentage (50% being the half of the layer)", InputAffix = "%")]
        public SKSizeLayerProperty CustomEmitterSize { get; set; }

        [PropertyDescription(Description = "The angle at which to spawn particles", InputAffix = "°")]
        public FloatLayerProperty Angle { get; set; }

        [PropertyDescription(Description = "How far to spread the particles out", InputAffix = "°", MinInputValue = 0f)]
        public FloatLayerProperty Spread { get; set; }

        protected override void PopulateDefaults()
        {
            ParticleRate.DefaultValue = 40;
            Angle.DefaultValue = -90f;
            Spread.DefaultValue = 15f;
        }

        protected override void EnableProperties()
        {
            CustomEmitterPosition.IsVisibleWhen(EmitterPosition, p => p.CurrentValue == PropertyGroups.EmitterPosition.Custom);
            CustomEmitterSize.IsVisibleWhen(EmitterPosition, p => p.CurrentValue == PropertyGroups.EmitterPosition.Custom);
        }

        protected override void DisableProperties()
        {
        }
    }

    public class ParticlesPropertyGroup : LayerPropertyGroup
    {
        [PropertyDescription]
        public ColorGradientLayerProperty Colors { get; set; }

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


        protected override void PopulateDefaults()
        {
            Colors.DefaultValue = new ColorGradient
            {
                Stops =
                {
                    new ColorGradientStop(new SKColor(255, 60, 0), 0f),
                    new ColorGradientStop(new SKColor(255, 128, 0), 0.5f),
                    new ColorGradientStop(new SKColor(255, 80, 0), 1f)
                }
            };

            InitialVelocity.DefaultValue = new FloatRange(100f, 200f);
            RotationVelocity.DefaultValue = new FloatRange(10f, 75f);
            MaximumVelocity.DefaultValue = 0f;
            Lifetime.DefaultValue = 2f;
            FadeOut.DefaultValue = true;
        }


        protected override void EnableProperties()
        {
        }

        protected override void DisableProperties()
        {
        }
    }

    public enum EmitterPosition
    {
        Top,
        Right,
        Bottom,
        Left,
        Center,
        Custom
    }
}