using System;
using System.Collections.Specialized;
using Artemis.Core;
using Artemis.Core.LayerBrushes;
using SkiaSharp;
using static Artemis.Plugins.LayerBrushes.Noise.FractalProperties;
using static FastNoiseLite;

namespace Artemis.Plugins.LayerBrushes.Noise
{
    public class NoiseBrush : PerLedLayerBrush<NoiseBrushProperties>
    {
        private static readonly Random Rand = new();
        private readonly FastNoiseLite _noise;
        private ColorGradient? _gradient;
        private SKColor[] _colorMap;
        private float _x;
        private float _y;
        private float _z;

        public NoiseBrush()
        {
            _x = Rand.Next(0, 4096);
            _y = Rand.Next(0, 4096);
            _z = Rand.Next(0, 4096);
            _noise = new FastNoiseLite(Rand.Next(0, 4096));
        }

        public override void EnableLayerBrush()
        {
            Properties.LayerPropertyOnCurrentValueSet += (_, _) => Update(0);
            Properties.Colors.GradientColor.PropertyChanged += (_, _) => UpdateGradient();
            
            UpdateGradient();
        }

        private void UpdateGradient()
        {
            if (_gradient != null)
            {
                _gradient.CollectionChanged -= GradientOnCollectionChanged;
                _gradient.StopChanged -= GradientOnStopChanged;
            }

            _gradient = Properties.Colors.GradientColor.BaseValue;
            _gradient.CollectionChanged += GradientOnCollectionChanged;
            _gradient.StopChanged += GradientOnStopChanged;
            CreateColorMap(_gradient);
        }

        public override void DisableLayerBrush()
        {
            if (_gradient != null)
            {
                _gradient.CollectionChanged -= GradientOnCollectionChanged;
                _gradient.StopChanged -= GradientOnStopChanged;
            }
        }

        public override void Update(double deltaTime)
        {
            _x += Properties.ScrollSpeed.CurrentValue.X * 10 * (float) deltaTime;
            _y += Properties.ScrollSpeed.CurrentValue.Y * 10 * (float) deltaTime;
            _z += Properties.AnimationSpeed.CurrentValue * 2 * (float) deltaTime;

            _noise.SetNoiseType(Properties.NoiseType);
            _noise.SetFractalType((FractalType) Properties.Fractal.FractalType.CurrentValue);

            // Fractal settings
            if (Properties.Fractal.FractalType.CurrentValue != PropertiesFractalType.None)
            {
                _noise.SetFractalOctaves(Properties.Fractal.Octaves);
                _noise.SetFractalLacunarity(Properties.Fractal.Lacunarity);
                _noise.SetFractalGain(Properties.Fractal.Gain);
                _noise.SetFractalWeightedStrength(Properties.Fractal.WeightedStrength);
            }

            if (Properties.Fractal.FractalType.CurrentValue == PropertiesFractalType.PingPong)
                _noise.SetFractalPingPongStrength(Properties.Fractal.PingPongStrength);

            // Cellular settings
            if (Properties.NoiseType.CurrentValue == NoiseType.Cellular)
            {
                _noise.SetCellularDistanceFunction(Properties.Cellular.DistanceFunction);
                _noise.SetCellularReturnType(Properties.Cellular.ReturnType);
                _noise.SetCellularJitter(Properties.Cellular.Jitter);
            }

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
            float scrolledX = renderPoint.X + _x;
            if (float.IsNaN(scrolledX) || float.IsInfinity(scrolledX))
                scrolledX = 0;
            float scrolledY = renderPoint.Y + _y;
            if (float.IsNaN(scrolledY) || float.IsInfinity(scrolledY))
                scrolledY = 0;

            SKSize scale = Properties.Scale.CurrentValue;
            float width = 1f / (MathF.Max(scale.Width, 0.001f) / 50f);
            float height = 1f / (MathF.Max(scale.Height, 0.001f) / 50f);

            float evalX = scrolledX * width;
            float evalY = scrolledY * height;

            // v should be between -1 and 1
            float v = Math.Clamp(_noise.GetNoise(evalX, evalY, _z), -1f, 1f);
            // normalize to between 0 and 1
            float amount = (v + 1f) / 2f;

            int segments = Properties.Colors.Segments.CurrentValue - 1;
            if (Properties.Colors.SegmentColors && segments > 0)
                amount = MathF.Round(amount * segments, MidpointRounding.ToEven) / segments;
            if (Properties.Colors.GradientColor.CurrentValue != null && _colorMap.Length == 100)
                return _colorMap[Math.Clamp((int) (amount * 100), 0, 99)];
            return SKColor.Empty;
        }
        
        private void GradientOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CreateColorMap(Properties.Colors.GradientColor.CurrentValue);
        }

        private void GradientOnStopChanged(object sender, EventArgs e)
        {
            CreateColorMap(Properties.Colors.GradientColor.CurrentValue);
        }

        private void CreateColorMap(ColorGradient gradient)
        {
            SKColor[] colorMap = new SKColor[100];
            for (int i = 0; i < 100; i++)
                colorMap[i] = gradient.GetColor(i / 99f);

            _colorMap = colorMap;
        }
    }

    public enum ColorMappingType
    {
        Simple,
        Gradient
    }
}