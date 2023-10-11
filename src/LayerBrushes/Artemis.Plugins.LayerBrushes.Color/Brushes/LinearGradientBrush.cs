using System.Linq;
using Artemis.Core;
using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.Color.PropertyGroups;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Color
{
    public class LinearGradientBrush : LayerBrush<LinearGradientBrushProperties>
    {
        private float _scrollX;
        private float _scrollY;

        /// <inheritdoc />
        public override void Render(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
            float waveSize = Properties.WaveSize / 100f;

            SKMatrix matrix = SKMatrix.Concat(
                SKMatrix.CreateRotationDegrees(Properties.Rotation, bounds.MidX, bounds.MidY),
                SKMatrix.CreateScale(waveSize, waveSize, bounds.MidX, bounds.MidY)
            );
            matrix = SKMatrix.Concat(
                matrix,
                SKMatrix.CreateTranslation(_scrollX * bounds.Width, _scrollY * bounds.Height)
            );
            
            // Render gradient
            ColorGradient gradient = Properties.Colors;
            using SKShader shader = SKShader.CreateLinearGradient(
                new SKPoint(bounds.Left, bounds.Top),
                new SKPoint(
                    Properties.Orientation == LinearGradientOrientationMode.Horizontal ? bounds.Right : bounds.Left,
                    Properties.Orientation == LinearGradientOrientationMode.Horizontal ? bounds.Top : bounds.Bottom
                ),
                gradient.Colors,
                gradient.Positions,
                Properties.TileMode.CurrentValue,
                matrix
            );

            paint.Shader = shader;
            canvas.DrawRect(bounds, paint);
            paint.Shader = null;
        }

        public override void EnableLayerBrush()
        {
        }

        public override void DisableLayerBrush()
        {
        }

        public override void Update(double deltaTime)
        {
            // Divide delta by 60 to achieve an RPM value
            float minuteDelta = (float) deltaTime / 60f;
            // Divide by the wave size to keep the gradient moving at the same pace regardless of wave size
            float waveSize = Properties.WaveSize / 100f;

            _scrollX += Properties.ScrollSpeed.CurrentValue.X * minuteDelta / waveSize;
            _scrollY += Properties.ScrollSpeed.CurrentValue.Y * minuteDelta / waveSize;

            _scrollX %= 1f;
            _scrollY %= 1f;
        }
    }
}