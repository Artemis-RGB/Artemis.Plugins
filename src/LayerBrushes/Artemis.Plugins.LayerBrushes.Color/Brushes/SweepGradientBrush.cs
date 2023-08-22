using System;
using System.Linq;
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
            SKPoint position = new(bounds.Left + bounds.Width * Properties.Position.CurrentValue.X, bounds.Top + bounds.Height * Properties.Position.CurrentValue.Y);
            using SKShader shader = SKShader.CreateSweepGradient(
                position,
                Properties.Colors.CurrentValue.Colors,
                Properties.Colors.CurrentValue.Positions,
                SKShaderTileMode.Clamp,
                Properties.StartAngle,
                Properties.EndAngle,
                SKMatrix.CreateRotationDegrees(_rotation, bounds.MidX, bounds.MidY)
            );

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
            _rotation += Properties.RotateSpeed * (float) deltaTime % 360f;
        }

        #endregion
    }
}