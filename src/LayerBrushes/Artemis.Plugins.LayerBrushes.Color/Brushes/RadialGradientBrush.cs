using System;
using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.Color.PropertyGroups;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Color
{
    public class RadialGradientBrush : LayerBrush<RadialGradientBrushProperties>
    {
        #region Overrides of LayerBrush<RadialGradientBrushProperties>

        /// <inheritdoc />
        public override void Render(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
            // TODO: Investigate performance
            SKPoint position = new SKPoint(
                bounds.MidX + bounds.MidX * (Properties.CenterOffset.CurrentValue.X / 100f),
                bounds.MidY + bounds.MidY * (Properties.CenterOffset.CurrentValue.Y / 100f)
            );

            paint.Shader = Properties.ResizeMode.CurrentValue switch
            {
                RadialGradientResizeMode.Fit => SKShader.CreateRadialGradient(
                    position,
                    Math.Min(bounds.Width, bounds.Height) / 2f,
                    Properties.Colors.BaseValue.GetColorsArray(Properties.ColorsMultiplier),
                    Properties.Colors.BaseValue.GetPositionsArray(Properties.ColorsMultiplier),
                    SKShaderTileMode.Clamp
                ),
                RadialGradientResizeMode.Fill => SKShader.CreateRadialGradient(
                    position,
                    Math.Max(bounds.Width, bounds.Height) / 2f,
                    Properties.Colors.BaseValue.GetColorsArray(Properties.ColorsMultiplier),
                    Properties.Colors.BaseValue.GetPositionsArray(Properties.ColorsMultiplier),
                    SKShaderTileMode.Clamp
                ),
                RadialGradientResizeMode.Stretch => SKShader.CreateRadialGradient(
                    new SKPoint(0, 0),
                    0.5f,
                    Properties.Colors.BaseValue.GetColorsArray(Properties.ColorsMultiplier),
                    Properties.Colors.BaseValue.GetPositionsArray(Properties.ColorsMultiplier),
                    SKShaderTileMode.Clamp,
                    SKMatrix.CreateScale(bounds.Width, bounds.Height, 0, 0).PostConcat(SKMatrix.CreateTranslation(position.X, position.Y))
                ),
                _ => throw new ArgumentOutOfRangeException()
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
        }

        #endregion
    }
}