using System;
using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.Color.PropertyGroups;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Color
{
    public class SolidBrush : LayerBrush<SolidBrushProperties>
    {
        private float _position;

        #region Overrides of LayerBrush<SolidBrushProperties>

        /// <inheritdoc />
        public override void Render(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
            if (Properties.EnableColorAnimation)
                paint.Shader = SKShader.CreateColor(Properties.Colors.CurrentValue.GetColor(_position));
            else
                paint.Shader = SKShader.CreateColor(Properties.Color);

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
            // Take 4 seconds for a full rotation at 100%
            float newPosition = _position + Properties.AnimationSpeed / 400f * (float) deltaTime;
            _position = newPosition - 1f * (float) Math.Floor(newPosition / 1f);
        }

        #endregion
    }
}