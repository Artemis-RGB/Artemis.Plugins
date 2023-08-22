using Artemis.Core;
using SkiaSharp;

namespace Artemis.Plugins.Input.LayerBrush.Keypress.Effects
{
    public class KeypressOnPress : IKeyPressEffect
    {
        private readonly KeypressBrush _brush;

        public KeypressOnPress(KeypressBrush brush, ArtemisLed led, SKPoint position)
        {
            _brush = brush;
            Led = led;
            Position = position;

            UpdatePaint();
        }

        public SKPaint Paint { get; set; }
        public float Size { get; set; }
        public float GrowthSpeed { get; set; } = 500;
        public float MaxSize { get; set; } = 100;
        public bool AllowDuplicates => true;
        public bool Finished => Size > MaxSize;
        public ArtemisLed Led { get; }
        public SKPoint Position { get; set; }
        
        private void UpdatePaint()
        {
            if (_brush.Properties.ColorMode.CurrentValue == ColorType.Random && Paint == null)
                Paint = new SKPaint { Color = SKColor.FromHsv(_brush.Rand.Next(0, 360), 100, 100) };
            else if (_brush.Properties.ColorMode.CurrentValue == ColorType.Solid)
            {
                Paint?.DisposeSelfAndProperties();
                Paint = new SKPaint { Color = _brush.Properties.Color.CurrentValue };
            }
            else if (_brush.Properties.ColorMode.CurrentValue == ColorType.Gradient)
            {
                Paint?.DisposeSelfAndProperties();
                Paint = new SKPaint
                {
                    Shader = SKShader.CreateRadialGradient(Position, Size, _brush.Properties.Colors.CurrentValue.Colors, _brush.Properties.Colors.CurrentValue.Positions, SKShaderTileMode.Clamp)
                };
            }
        }
        
        public void Update(double deltaTime)
        {
        }

        public void Render(SKCanvas canvas)
        {
        }

        public void Respawn()
        {
        }

        public void Despawn()
        {
        }
    }
}