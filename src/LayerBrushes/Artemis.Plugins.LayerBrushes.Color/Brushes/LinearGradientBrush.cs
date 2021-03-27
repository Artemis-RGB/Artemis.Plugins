using Artemis.Core;
using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.Color.PropertyGroups;
using SkiaSharp;
using System;
using System.Diagnostics;
using System.Linq;

namespace Artemis.Plugins.LayerBrushes.Color
{
    public class LinearGradientBrush : LayerBrush<LinearGradientBrushProperties>
    {
        private float _scrollX;
        private float _scrollY;
        private ColorGradient _seamlessColorGradient;
        private ColorGradient _colorGradient;
        private SKShaderTileMode _sKShaderTileMode;

        #region Overrides of LayerBrush<LinearGradientBrushProperties>

        /// <inheritdoc />
        public override void Render(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
            SKMatrix matrix = SKMatrix.Concat(
                SKMatrix.CreateTranslation(_scrollX, _scrollY),
                SKMatrix.CreateRotationDegrees(Properties.Rotation, bounds.MidX, bounds.MidY)
            );

            if (Properties.RepeatMode.CurrentValue == LinearGradientRepeatmode.RepeatSeamless)
            {
                if (_seamlessColorGradient == null)
                    _seamlessColorGradient = GetSeamlessGradient(Properties.Colors.BaseValue);

                _sKShaderTileMode = SKShaderTileMode.Repeat;
                _colorGradient = _seamlessColorGradient;
            }
            else if (Properties.RepeatMode.CurrentValue == LinearGradientRepeatmode.Mirror)
            {
                _sKShaderTileMode = SKShaderTileMode.Mirror;
                _colorGradient = Properties.Colors.BaseValue;
            }
            else
            {
                _sKShaderTileMode = SKShaderTileMode.Repeat;
                _colorGradient = Properties.Colors.BaseValue;
            }

            //Render gradient
            paint.Shader = SKShader.CreateLinearGradient(
            new SKPoint(bounds.Left, bounds.Top),
            new SKPoint(
                ((Properties.Orientation == LinearGradientOrientatonMode.Horizontal) ? bounds.Right : bounds.Left) * Properties.WaveSize / 100,
                ((Properties.Orientation == LinearGradientOrientatonMode.Horizontal) ? bounds.Top : bounds.Bottom) * Properties.WaveSize / 100
                ),
            _colorGradient.GetColorsArray(),
            _colorGradient.GetPositionsArray(),
            _sKShaderTileMode,
            matrix
            );

            canvas.DrawRect(bounds, paint);
            paint.Shader?.Dispose();
            paint.Shader = null;
        }

        /// <summary>
        /// This method takes a Color gradient as input to create a new seamless gradient by compressing existing stops and adding a new final stop with same colors as the initial stop while keeping distance relations.
        /// </summary>
        /// <param name="sourceGradient">Source gradient to create a new seamless gradient</param>
        /// <returns>Seamless gradient</returns>
        private ColorGradient GetSeamlessGradient(ColorGradient sourceGradient)
        {

            if (sourceGradient.Stops.Count < 2)
            {
                return sourceGradient;
            }

            //Create a new seamless gradient.
            ColorGradient seamlessGradient = new ColorGradient();

            ColorGradientStop firstStop = sourceGradient.Stops.OrderBy(stop => stop.Position).ToArray()[0];
            ColorGradientStop secondStop = sourceGradient.Stops.OrderBy(stop => stop.Position).ToArray()[1];

            //Get compress ratio to keep the gradient step ratios.
            float compressionRatio = 1f - (secondStop.Position - firstStop.Position);

            //Fix ratio if there is only two steps at 0 and 1
            if (compressionRatio == 0f)
                compressionRatio = 0.66f;

            //Fill new seamless gradient
            foreach (ColorGradientStop stop in sourceGradient.Stops)
            {
                seamlessGradient.Stops.Add(new ColorGradientStop(
                    stop.Color,
                    stop.Position * compressionRatio
                    ));
            }

            //Add the seamless end stop
            seamlessGradient.Stops.Add(new ColorGradientStop(
                firstStop.Color,
                1f
                ));

            return seamlessGradient;
        }

        #endregion

        #region Overrides of BaseLayerBrush

        public override void EnableLayerBrush()
        {
            Properties.Colors.BaseValue.PropertyChanged += BaseValue_PropertyChanged;
        }
        private void BaseValue_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Properties.RepeatMode.CurrentValue != LinearGradientRepeatmode.RepeatSeamless)
                return;

            _seamlessColorGradient = GetSeamlessGradient(Properties.Colors.BaseValue);
            Debug.WriteLine("Update gradient");
        }

        public override void DisableLayerBrush()
        {
            Properties.Colors.BaseValue.PropertyChanged -= BaseValue_PropertyChanged;
        }

        public override void Update(double deltaTime)
        {
            _scrollX += Properties.ScrollSpeed.CurrentValue.X * 10 * (float)deltaTime;
            _scrollY += Properties.ScrollSpeed.CurrentValue.Y * 10 * (float)deltaTime;
        }

        #endregion
    }
}