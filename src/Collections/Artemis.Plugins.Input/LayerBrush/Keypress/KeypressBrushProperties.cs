using Artemis.Core;
using SkiaSharp;

namespace Artemis.Plugins.Input.LayerBrush.Keypress
{
    public class KeypressBrushProperties : LayerPropertyGroup
    {
        // Common

        public EnumLayerProperty<AnimationType> Animation { get; set; }

        public EnumLayerProperty<ColorType> ColorMode { get; set; }

        [PropertyDescription(Description = "The gradient of the circle or the colors to cycle through in the echo.")]
        public ColorGradientLayerProperty Colors { get; set; }

        public SKColorLayerProperty Color { get; set; }

        // Echo

        [PropertyDescription(Description = "If enabled, the echo fades out after you release the key.")]
        public BoolLayerProperty FadeEcho { get; set; }

        [PropertyDescription(Description = "The lifetime of the echo in seconds after you release the key.", InputAffix = "s")]
        public FloatLayerProperty EchoLifetime { get; set; }

        // Ripple

        [PropertyDescription(Description = "Set how ripple will respond to a keypress.")]
        public EnumLayerProperty<RippleBehivor> RippleBehivor { get; set; }

        [PropertyDescription(Description = "Fade away ripple effect mode.")]
        public EnumLayerProperty<RippleFadeOutMode> RippleFadeAway { get; set; }

        [PropertyDescription(Description = "Width of the ripple.")]
        public FloatLayerProperty RippleWidth { get; set; }

        [PropertyDescription(Description = "Size of the expand area of the ripple.")]
        public FloatLayerProperty RippleSize { get; set; }

        [PropertyDescription(Description = "Growth speed of the ripple.")]
        public FloatLayerProperty RippleGrowthSpeed { get; set; }

        // Circle

        [PropertyDescription(Description = "Expand speed of the ripple.")]
        public FloatLayerProperty CircleSize { get; set; }

        [PropertyDescription(Description = "Expand speed of the ripple.")]
        public FloatLayerProperty CircleGrowthSpeed { get; set; }



        protected override void PopulateDefaults()
        {
            Color.DefaultValue = new SKColor(255, 0, 0);
            Colors.DefaultValue = new ColorGradient();
            Colors.DefaultValue.Add(new ColorGradientStop(new SKColor(255, 255, 0, 255), 0.0f));
            Colors.DefaultValue.Add(new ColorGradientStop(new SKColor(255, 0, 0, 255), 0.7f));
            Colors.DefaultValue.Add(new ColorGradientStop(new SKColor(255, 0, 0, 255), 0.85f));
            Colors.DefaultValue.Add(new ColorGradientStop(new SKColor(255, 0, 0, 0), 1.0f));

            //Echo
            FadeEcho.DefaultValue = true;
            EchoLifetime.DefaultValue = 1;

            //Ripple
            RippleWidth.DefaultValue = 20;
            RippleSize.DefaultValue = 100;
            RippleGrowthSpeed.DefaultValue = 300;
            RippleBehivor.DefaultValue = Keypress.RippleBehivor.OneAtATime;
            RippleFadeAway.DefaultValue = RippleFadeOutMode.Sine;

            //Circle
            CircleSize.DefaultValue = 100;
            CircleGrowthSpeed.DefaultValue = 500;
        }

        protected override void EnableProperties()
        {
            // Shared
            Colors.IsVisibleWhen(ColorMode, c => c.CurrentValue == ColorType.Gradient);
            Color.IsVisibleWhen(ColorMode, c => c.CurrentValue == ColorType.Solid);

            // Echo
            FadeEcho.IsVisibleWhen(Animation, a => a.CurrentValue == AnimationType.Echo);
            EchoLifetime.IsVisibleWhen(Animation, a => a.CurrentValue == AnimationType.Echo);

            //Ripple
            RippleWidth.IsVisibleWhen(Animation, a => a.CurrentValue == AnimationType.Ripple);
            RippleFadeAway.IsVisibleWhen(Animation, a => a.CurrentValue == AnimationType.Ripple);
            RippleSize.IsVisibleWhen(Animation, a => a.CurrentValue == AnimationType.Ripple);
            RippleGrowthSpeed.IsVisibleWhen(Animation, a => a.CurrentValue == AnimationType.Ripple);
            RippleBehivor.IsVisibleWhen(Animation, a => a.CurrentValue == AnimationType.Ripple);

            //Circle
            CircleSize.IsVisibleWhen(Animation, a => a.CurrentValue == AnimationType.CircleWhilePressed);
            CircleGrowthSpeed.IsVisibleWhen(Animation, a => a.CurrentValue == AnimationType.CircleWhilePressed);
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

    public enum RippleFadeOutMode
    {
        None = -1,
        Linear = 0,
        Sine = 14
    }

    public enum AnimationType
    {
        CircleWhilePressed = 0,
        Ripple = 1,
        Echo = 2
    }
    public enum RippleBehivor
    {
        OneAtATime = 0,
        ResetCurrentRipple = 1,
        CreateNewRipple = 2,
        ContinuousWhileKeyPressed = 3,
    }
}