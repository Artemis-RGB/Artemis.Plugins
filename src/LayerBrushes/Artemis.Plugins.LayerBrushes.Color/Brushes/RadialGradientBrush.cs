using System;
using System.Collections.Generic;
using System.Linq;
using Artemis.Core;
using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.Color.PropertyGroups;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Color
{
    public class RadialGradientBrush : LayerBrush<RadialGradientBrushProperties>
    {
        private float _progress = 0;

        #region Overrides of LayerBrush<RadialGradientBrushProperties>

        /// <inheritdoc />
        public override void Render(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
            SKColor[] colors;
            float[] positions;

            // Get the stops directly if zooming is not used in any way
            if (Properties.ZoomSpeed == 0 && _progress == 0)
            {
                colors = Properties.Colors.CurrentValue.Colors;
                positions = Properties.Colors.CurrentValue.Positions;
            }
            // Get the animated stops and split them up into usable arrays
            else
            {
                List<ColorGradientStop> stops = GetAnimatedStops(_progress);
                colors = stops.Select(s => s.Color).ToArray();
                positions = stops.Select(s => s.Position).ToArray();
            }

            SKPoint position = new(bounds.Left + bounds.Width * Properties.Position.CurrentValue.X, bounds.Top + bounds.Height * Properties.Position.CurrentValue.Y);
            using SKShader shader =  Properties.ResizeMode.CurrentValue switch
            {
                RadialGradientResizeMode.Fit => SKShader.CreateRadialGradient(position, Math.Min(bounds.Width, bounds.Height) / 2f, colors, positions, SKShaderTileMode.Clamp),
                RadialGradientResizeMode.Fill => SKShader.CreateRadialGradient(position, Math.Max(bounds.Width, bounds.Height) / 2f, colors, positions, SKShaderTileMode.Clamp),
                RadialGradientResizeMode.Stretch => SKShader.CreateRadialGradient(
                    new SKPoint(0, 0),
                    0.5f,
                    colors,
                    positions,
                    SKShaderTileMode.Clamp,
                    SKMatrix.CreateScale(bounds.Width, bounds.Height, 0, 0).PostConcat(SKMatrix.CreateTranslation(position.X, position.Y))
                ),
                _ => throw new ArgumentOutOfRangeException()
            };

            paint.Shader = shader;
            canvas.DrawRect(bounds, paint);
            paint.Shader = null;
        }

        #endregion

        #region Overrides of BaseLayerBrush

        public override void EnableLayerBrush()
        {
        }

        public override void DisableLayerBrush()
        {
        }

        public override void Update(double deltaTime)
        {
            double toAdd = deltaTime * Properties.ZoomSpeed;
            _progress = Modulo((float) (_progress + toAdd), 1);
        }

        #endregion

        private List<ColorGradientStop> GetAnimatedStops(float animationProgress)
        {
            // Never use the stops directly as their positions are modified below
            float[] positions = Properties.Colors.CurrentValue.Positions;
            SKColor[] colors = Properties.Colors.CurrentValue.Colors;
            List<ColorGradientStop> stops = positions.Select((p, i) => new ColorGradientStop(colors[i], p)).ToList();

            // Move position 0 forwards to avoid it X-fighting with 1 after applying progress
            if (stops[0].Position == 0)
                stops[0].Position = 0.001f;

            foreach (ColorGradientStop colorGradientStop in stops)
                colorGradientStop.Position = (colorGradientStop.Position + animationProgress) % 1;

            stops = stops.OrderBy(s => s.Position).ToList();

            // Add current start of the gradient to 0.0 and the end of the gradient to 1.0
            stops.Insert(0, new ColorGradientStop(Properties.Colors.CurrentValue.GetColor(1f - _progress), 0));
            stops.Add(new ColorGradientStop(Properties.Colors.CurrentValue.GetColor(1f - _progress), 1));

            return stops;
        }

        private static float Modulo(float a, float b)
        {
            return a - b * MathF.Floor(a / b);
        }
    }
}