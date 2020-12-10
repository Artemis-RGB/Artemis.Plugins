using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.Color.PropertyGroups;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Color
{
    public class LinearGradientBrush : LayerBrush<LinearGradientBrushProperties>
    {
        #region Overrides of LayerBrush<LinearGradientBrushProperties>

        /// <inheritdoc />
        public override void Render(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
            // TODO: Investigate performance
            paint.Shader = SKShader.CreateLinearGradient(
                new SKPoint(bounds.Left, bounds.Top),
                new SKPoint(bounds.Right, bounds.Top),
                Properties.Colors.BaseValue.GetColorsArray(Properties.ColorsMultiplier),
                Properties.Colors.BaseValue.GetPositionsArray(Properties.ColorsMultiplier),
                SKShaderTileMode.Repeat,
                SKMatrix.CreateRotationDegrees(Properties.Rotation, bounds.MidX, bounds.MidY)
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
        }

        #endregion
    }
}