using System;
using System.ComponentModel;
using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.Color.PropertyGroups;
using SkiaSharp;
using static Artemis.Plugins.LayerBrushes.Color.PropertyGroups.ColorBrushProperties;

namespace Artemis.Plugins.LayerBrushes.Color
{
    public class ColorBrush : LayerBrush<ColorBrushProperties>
    {
        private SKColor _color;
        private float _linearGradientRotation;
        private SKPaint _paint;
        private SKShader _shader;
        private SKRect _shaderBounds;

        public override void EnableLayerBrush()
        {
            Layer.RenderPropertiesUpdated += HandleShaderChange;
            Properties.LayerPropertyOnCurrentValueSet += HandleShaderChange;
            Properties.Colors.BaseValue.PropertyChanged += BaseValueOnPropertyChanged;
        }

        public override void DisableLayerBrush()
        {
            Layer.RenderPropertiesUpdated -= HandleShaderChange;
            Properties.LayerPropertyOnCurrentValueSet -= HandleShaderChange;
            Properties.Colors.BaseValue.PropertyChanged -= BaseValueOnPropertyChanged;

            _paint?.Dispose();
            _shader?.Dispose();
            _paint = null;
            _shader = null;
        }

        public override void Update(double deltaTime)
        {
            // While rendering a solid, if the color was changed since the last frame, recreate the shader
            if (Properties.GradientType.BaseValue == ColorType.Solid && _color != Properties.Color.CurrentValue)
                CreateSolid();
            // While rendering a linear gradient, if the rotation was changed since the last frame, recreate the shader
            else if (Properties.GradientType.BaseValue == ColorType.LinearGradient && Math.Abs(_linearGradientRotation - Properties.LinearGradientRotation.CurrentValue) > 0.01)
                CreateLinearGradient();
        }

        public override void Render(SKCanvas canvas, SKPath path, SKPaint paint)
        {
            if (path.Bounds != _shaderBounds)
            {
                _shaderBounds = path.Bounds;
                CreateShader();
            }

            paint.Shader = _shader;
            canvas.DrawPath(path, paint);
        }

        private void BaseValueOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CreateShader();
        }

        private void HandleShaderChange(object sender, EventArgs e)
        {
            CreateShader();
        }

        private void CreateShader()
        {
            switch (Properties.GradientType.CurrentValue)
            {
                case ColorType.Solid:
                    CreateSolid();
                    break;
                case ColorType.LinearGradient:
                    CreateLinearGradient();
                    break;
                case ColorType.RadialGradient:
                    CreateRadialGradient();
                    break;
                case ColorType.SweepGradient:
                    CreateSweepGradient();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdatePaint()
        {
            if (_paint == null)
                _paint = new SKPaint {Shader = _shader, FilterQuality = SKFilterQuality.Low};
            else
                _paint.Shader = _shader;
        }

        private void CreateSolid()
        {
            _color = Properties.Color.CurrentValue;

            _shader?.Dispose();
            _shader = SKShader.CreateColor(_color);
            UpdatePaint();
        }

        private void CreateLinearGradient()
        {
            int repeat = Properties.ColorsMultiplier.CurrentValue;
            _linearGradientRotation = Properties.LinearGradientRotation.CurrentValue;

            _shader?.Dispose();
            _shader = SKShader.CreateLinearGradient(
                new SKPoint(_shaderBounds.Left, _shaderBounds.Top),
                new SKPoint(_shaderBounds.Right, _shaderBounds.Top),
                Properties.Colors.BaseValue.GetColorsArray(repeat),
                Properties.Colors.BaseValue.GetPositionsArray(repeat),
                SKShaderTileMode.Clamp,
                SKMatrix.MakeRotationDegrees(_linearGradientRotation, _shaderBounds.Left, _shaderBounds.MidY)
            );
            UpdatePaint();
        }

        private void CreateRadialGradient()
        {
            int repeat = Properties.ColorsMultiplier.CurrentValue;

            _shader?.Dispose();

            SKPoint position = new SKPoint(
                _shaderBounds.MidX + _shaderBounds.MidX * (Properties.RadialGradient.CenterOffset.CurrentValue.X / 100f),
                _shaderBounds.MidY + _shaderBounds.MidY * (Properties.RadialGradient.CenterOffset.CurrentValue.Y / 100f)
            );

            switch (Properties.RadialGradient.ResizeMode.CurrentValue)
            {
                case RadialGradientProperties.RadialGradientResizeMode.Fit:
                    _shader = SKShader.CreateRadialGradient(
                        position,
                        Math.Min(_shaderBounds.Width, _shaderBounds.Height) / 2f,
                        Properties.Colors.BaseValue.GetColorsArray(repeat),
                        Properties.Colors.BaseValue.GetPositionsArray(repeat),
                        SKShaderTileMode.Clamp
                    );
                    break;
                case RadialGradientProperties.RadialGradientResizeMode.Fill:
                    _shader = SKShader.CreateRadialGradient(
                        position,
                        Math.Max(_shaderBounds.Width, _shaderBounds.Height) / 2f,
                        Properties.Colors.BaseValue.GetColorsArray(repeat),
                        Properties.Colors.BaseValue.GetPositionsArray(repeat),
                        SKShaderTileMode.Clamp
                    );
                    break;
                case RadialGradientProperties.RadialGradientResizeMode.Stretch:
                    _shader = SKShader.CreateRadialGradient(
                        new SKPoint(0, 0),
                        0.5f,
                        Properties.Colors.BaseValue.GetColorsArray(repeat),
                        Properties.Colors.BaseValue.GetPositionsArray(repeat),
                        SKShaderTileMode.Clamp,
                        SKMatrix.CreateScale(_shaderBounds.Width, _shaderBounds.Height, 0, 0).PostConcat(SKMatrix.CreateTranslation(position.X, position.Y))
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            UpdatePaint();
        }

        private void CreateSweepGradient()
        {
            int repeat = Properties.ColorsMultiplier.CurrentValue;

            _shader?.Dispose();
            _shader = SKShader.CreateSweepGradient(
                new SKPoint(_shaderBounds.MidX, _shaderBounds.MidY),
                Properties.Colors.BaseValue.GetColorsArray(repeat),
                Properties.Colors.BaseValue.GetPositionsArray(repeat),
                SKShaderTileMode.Clamp,
                0,
                360
            );
            UpdatePaint();
        }
    }
}