using System;
using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.Color.PropertyGroups;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Color
{
    public class SolidBrush : LayerBrush<SolidBrushProperties>
    {
        private float _animationPosition;

        #region Overrides of LayerBrush<SolidBrushProperties>

        /// <inheritdoc />
        public override void Render(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
            paint.Shader = Properties.ColorMode.CurrentValue switch
            {
                SolidBrushColorMode.Static => SKShader.CreateColor(Properties.Color),
                SolidBrushColorMode.GradientPosition => SKShader.CreateColor(Properties.Colors.CurrentValue.GetColor(Properties.GradientPosition / 100f)),
                SolidBrushColorMode.GradientAnimation => SKShader.CreateColor(Properties.Colors.CurrentValue.GetColor(_animationPosition)),
                _ => SKShader.CreateColor(Properties.Color)
            };

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
            // Take 1 sec for a full rotation at 100%
            float newPosition = _animationPosition + Properties.AnimationSpeed / 100f * (float) deltaTime;
            _animationPosition = newPosition - 1f * (float) Math.Floor(newPosition / 1f);
        }

        #endregion
    }
}