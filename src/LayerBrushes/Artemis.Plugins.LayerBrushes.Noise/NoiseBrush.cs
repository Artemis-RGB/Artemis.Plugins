using System;
using System.ComponentModel;
using Artemis.Core;
using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.Noise.Utilities;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Noise
{
    public class NoiseBrush : PerLedLayerBrush<NoiseBrushProperties>
    {
        private static readonly Random Rand = new Random();
        private SKColor[] _colorMap;
        private OpenSimplexNoise _noise;
        private float _x;
        private float _y;
        private float _z;

        public override void EnableLayerBrush()
        {
            _x = Rand.Next(0, 4096);
            _y = Rand.Next(0, 4096);
            _z = Rand.Next(0, 4096);
            _noise = new OpenSimplexNoise(Rand.Next(0, 4096));

            Properties.GradientColor.BaseValue.PropertyChanged += GradientColorChanged;
            CreateColorMap();
        }

        public override void DisableLayerBrush()
        {
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
            float hardness = Properties.Hardness.CurrentValue / 100f;

            float scrolledX = renderPoint.X + _x;
            if (float.IsNaN(scrolledX) || float.IsInfinity(scrolledX))
                scrolledX = 0;
            float scrolledY = renderPoint.Y + _y;
            if (float.IsNaN(scrolledY) || float.IsInfinity(scrolledY))
                scrolledY = 0;
            
            float evalX = scrolledX * (scale.Width * -1) / 1000f;
            float evalY = scrolledY * (scale.Height * -1) / 1000f;

            float v = (float) _noise.Evaluate(evalX, evalY, _z) * hardness;
            float amount = Math.Max(0f, Math.Min(1f, v));

            if (Properties.ColorType.BaseValue == ColorMappingType.Simple)
                return mainColor.Interpolate(secondColor, amount);
            if (gradientColor != null && _colorMap.Length == 101)
                return _colorMap[(int) Math.Round(amount * 100, MidpointRounding.AwayFromZero)];
            return SKColor.Empty;
        }

        private void GradientColorChanged(object sender, PropertyChangedEventArgs e)
        {
            CreateColorMap();
        }

        private void CreateColorMap()
        {
            SKColor[] colorMap = new SKColor[101];
            for (int i = 0; i < 101; i++)
                colorMap[i] = Properties.GradientColor.BaseValue.GetColor(i / 100f);

            _colorMap = colorMap;
        }
    }

    public enum ColorMappingType
    {
        Simple,
        Gradient
    }
}