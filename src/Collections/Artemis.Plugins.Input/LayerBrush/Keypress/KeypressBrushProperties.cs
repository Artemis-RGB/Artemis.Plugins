using Artemis.Core;
using SkiaSharp;

namespace Artemis.Plugins.Input.LayerBrush.Keypress
{
    public class KeypressBrushProperties : LayerPropertyGroup
    {
        public EnumLayerProperty<AnimationType> Animation { get; set; }

        public EnumLayerProperty<ColorType> ColorMode { get; set; }

        [PropertyDescription(Description = "The gradient of the circle or the colors to cycle through in the echo.")]
        public ColorGradientLayerProperty Colors { get; set; }

        public SKColorLayerProperty Color { get; set; }

        [PropertyDescription(Description = "If enabled, the echo fades out after you release the key.")]
        public BoolLayerProperty FadeEcho { get; set; }

        [PropertyDescription(Description = "The lifetime of the echo in seconds after you release the key.", InputAffix = "s")]
        public FloatLayerProperty EchoLifetime { get; set; }

        protected override void PopulateDefaults()
        {
            Color.DefaultValue = new SKColor(255, 0, 0);
            Colors.DefaultValue = new ColorGradient();
            Colors.DefaultValue.Add(new ColorGradientStop(new SKColor(255, 255, 0, 255), 0.0f));
            Colors.DefaultValue.Add(new ColorGradientStop(new SKColor(255, 0, 0, 255), 0.7f));
            Colors.DefaultValue.Add(new ColorGradientStop(new SKColor(255, 0, 0, 255), 0.85f));
            Colors.DefaultValue.Add(new ColorGradientStop(new SKColor(255, 0, 0, 0), 1.0f));

            FadeEcho.DefaultValue = true;
            EchoLifetime.DefaultValue = 1;
        }

        protected override void EnableProperties()
        {
            // Shared
            Colors.IsVisibleWhen(ColorMode, c => c.CurrentValue == ColorType.Gradient);
            Color.IsVisibleWhen(ColorMode, c => c.CurrentValue == ColorType.Solid);
            // Echo
            FadeEcho.IsVisibleWhen(Animation, a => a.CurrentValue == AnimationType.Echo);
            EchoLifetime.IsVisibleWhen(Animation, a => a.CurrentValue == AnimationType.Echo);
        }

        protected override void DisableProperties()
        {
        }
    }

    public enum ColorType
    {
        Random,
        Solid,
        Gradient
    }

    public enum AnimationType
    {
        CircleWhilePressed = 0,
        // CircleOnPress = 1,
        Echo = 2
    }
}