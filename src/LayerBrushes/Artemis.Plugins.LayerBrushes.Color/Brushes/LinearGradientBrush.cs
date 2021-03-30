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

        #region Overrides of LayerBrush<LinearGradientBrushProperties>

        /// <inheritdoc />
        public override void Render(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
            // For brevity's sake
            ColorGradient gradient = Properties.Colors.BaseValue;
            
            SKMatrix matrix = SKMatrix.Concat(
                SKMatrix.CreateTranslation(_scrollX, _scrollY),
                SKMatrix.CreateRotationDegrees(Properties.Rotation, bounds.MidX, bounds.MidY)
            );
            
            // LinearGradientRepeatMode.Mirror is currently the only setting that requires a different tile mode
            SKShaderTileMode tileMode = Properties.RepeatMode.CurrentValue == LinearGradientRepeatMode.Mirror
                ? SKShaderTileMode.Mirror
                : SKShaderTileMode.Repeat;

            // Render gradient
            paint.Shader = SKShader.CreateLinearGradient(
                new SKPoint(bounds.Left, bounds.Top),
                new SKPoint(
                    (Properties.Orientation == LinearGradientOrientationMode.Horizontal ? bounds.Right : bounds.Left) * Properties.WaveSize / 100,
                    (Properties.Orientation == LinearGradientOrientationMode.Horizontal ? bounds.Top : bounds.Bottom) * Properties.WaveSize / 100
                ),
                gradient.GetColorsArray(0, Properties.RepeatMode.CurrentValue == LinearGradientRepeatMode.RepeatSeamless),
                gradient.GetPositionsArray(0, Properties.RepeatMode.CurrentValue == LinearGradientRepeatMode.RepeatSeamless),
                tileMode,
                matrix
            );

            canvas.DrawRect(bounds, paint);
            paint.Shader?.Dispose();
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
            _scrollX += Properties.ScrollSpeed.CurrentValue.X * 10 * (float) deltaTime;
            _scrollY += Properties.ScrollSpeed.CurrentValue.Y * 10 * (float) deltaTime;
        }

        #endregion
    }
}