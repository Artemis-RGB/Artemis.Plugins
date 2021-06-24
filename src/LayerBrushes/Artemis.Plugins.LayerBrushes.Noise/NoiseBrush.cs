using System;
using System.Collections.Specialized;
using Artemis.Core;
using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.Noise.Utilities;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Noise
{
    public class NoiseBrush : PerLedLayerBrush<NoiseBrushProperties>
    {
        private static readonly Random Rand = new();
        private readonly OpenSimplex2S _noise;
        private SKColor[] _colorMap;
        private float _x;
        private float _y;
        private float _z;

        public NoiseBrush()
        {
            _x = Rand.Next(0, 4096);
            _y = Rand.Next(0, 4096);
            _z = Rand.Next(0, 4096);
            _noise = new OpenSimplex2S(Rand.Next(0, 4096));
        }

        public override void EnableLayerBrush()
        {
            Properties.GradientColor.BaseValue.CollectionChanged += GradientOnCollectionChanged;
            CreateColorMap();
        }

        public override void DisableLayerBrush()
        {
            Properties.GradientColor.BaseValue.CollectionChanged -= GradientOnCollectionChanged;
        }

        public override void Update(double deltaTime)
        {
            _x += Properties.ScrollSpeed.CurrentValue.X / 500f / (float) deltaTime;
            _y += Properties.ScrollSpeed.CurrentValue.Y / 500f / (float) deltaTime;
            _z += Properties.AnimationSpeed.CurrentValue / 500f / 0.04f * (float) deltaTime;

            // A telltale sign of someone who can't do math very well
            if (float.IsPositiveInfinity(_x) || float.IsNegativeInfinity(_x) || float.IsNaN(_x))
                _x = 0;
            if (float.IsPositiveInfinity(_y) || float.IsNegativeInfinity(_y) || float.IsNaN(_y))
                _y = 0;
            if (float.IsPositiveInfinity(_z) || float.IsNegativeInfinity(_z) || float.IsNaN(_z))
                _z = 0;
        }

        public override SKColor GetColor(ArtemisLed led, SKPoint renderPoint)
        {
            SKColor mainColor = Properties.MainColor.CurrentValue;
            SKColor secondColor = Properties.SecondaryColor.CurrentValue;
            ColorGradient gradientColor = Properties.GradientColor.CurrentValue;
            SKSize scale = Properties.Scale.CurrentValue;
            int segments = Properties.Segments.CurrentValue - 1;

            float scrolledX = renderPoint.X + _x;
            if (float.IsNaN(scrolledX) || float.IsInfinity(scrolledX))
                scrolledX = 0;
            float scrolledY = renderPoint.Y + _y;
            if (float.IsNaN(scrolledY) || float.IsInfinity(scrolledY))
                scrolledY = 0;

            float width = 1f / (MathF.Max(scale.Width, 0.001f) / 25f);
            float height = 1f / (MathF.Max(scale.Height, 0.001f) / 25f);

            float evalX = scrolledX * (width / 40f);
            float evalY = scrolledY * (height / 40f);

            // v should be between -1 and 1
            float v = (float) _noise.Noise3_Classic(evalX, evalY, _z);
            // normalize to between 0 and 1
            float amount = (Math.Clamp(v, -1f, 1f) + 1f) / 2f;

            if (Properties.SegmentColors && segments > 0)
                amount = MathF.Round(amount * segments, MidpointRounding.ToEven) / segments;

            if (Properties.ColorType.BaseValue == ColorMappingType.Simple)
                return mainColor.Interpolate(secondColor, amount);
            if (gradientColor != null && _colorMap.Length == 100)
                return _colorMap[Math.Clamp((int) (amount * 100), 0, 99)];
            return SKColor.Empty;
        }

        private void GradientOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CreateColorMap();
        }

        private void CreateColorMap()
        {
            SKColor[] colorMap = new SKColor[100];
            for (int i = 0; i < 100; i++)
                colorMap[i] = Properties.GradientColor.BaseValue.GetColor(i / 99f);

            _colorMap = colorMap;
        }
    }

    public enum ColorMappingType
    {
        Simple,
        Gradient
    }
}