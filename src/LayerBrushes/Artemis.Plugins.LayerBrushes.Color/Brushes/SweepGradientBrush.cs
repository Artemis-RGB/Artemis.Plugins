using System;
using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.Color.PropertyGroups;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Color
{
    public class SweepGradientBrush : LayerBrush<SweepGradientBrushProperties>
    {
        private float _rotation;

        #region Overrides of LayerBrush<SweepGradientBrushProperties>

        /// <inheritdoc />
        public override void Render(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
            // TODO: Investigate performance
            paint.Shader = SKShader.CreateSweepGradient(
                new SKPoint(bounds.Width * Properties.Position.CurrentValue.X, bounds.Height * Properties.Position.CurrentValue.Y),
                Properties.Colors.BaseValue.GetColorsArray(Properties.ColorsMultiplier),
                Properties.Colors.BaseValue.GetPositionsArray(Properties.ColorsMultiplier),
                SKShaderTileMode.Clamp,
                Properties.StartAngle,
                Properties.EndAngle,
                SKMatrix.CreateRotationDegrees(_rotation, bounds.MidX, bounds.MidY)
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
            _rotation += Properties.RotateSpeed * (float) deltaTime % 360f;
        }

        #endregion
    }
}