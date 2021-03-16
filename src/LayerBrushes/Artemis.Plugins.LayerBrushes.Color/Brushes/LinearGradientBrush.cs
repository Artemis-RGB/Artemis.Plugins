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
            SKMatrix matrix = SKMatrix.Concat(
                SKMatrix.CreateTranslation(_scrollX, _scrollY),
                SKMatrix.CreateRotationDegrees(Properties.Rotation, bounds.MidX, bounds.MidY)
            );

            paint.Shader = SKShader.CreateLinearGradient(
                new SKPoint(bounds.Left, bounds.Top),
                new SKPoint(
                    ((Properties.Orientation == LinearGradientOrientatonMode.Horizontal) ? bounds.Right : bounds.Left) * Properties.WaveSize / 100,
                    ((Properties.Orientation == LinearGradientOrientatonMode.Horizontal) ? bounds.Top : bounds.Bottom) * Properties.WaveSize / 100
                    ),
                Properties.Colors.BaseValue.GetColorsArray(0),
                Properties.Colors.BaseValue.GetPositionsArray(0),
                SKShaderTileMode.Repeat,
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
            _scrollX += Properties.ScrollSpeed.CurrentValue.X * 10 * (float)deltaTime;
            _scrollY += Properties.ScrollSpeed.CurrentValue.Y * 10 * (float)deltaTime;
        }

        #endregion
    }
}