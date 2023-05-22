using System;
using SkiaSharp;

namespace Artemis.Plugins.LayerEffects.Filter.Utilities
{
    public class ColorFilterGenerator
    {
        public static SKColorFilter GetHueColorMatrix(float value)
        {
            value = (float) (Math.PI / 180 * value);

            float cos = (float) Math.Cos(value);
            float sin = (float) Math.Sin(value);

            float[] mat =
            {
                0.213f + cos * 0.787f - sin * 0.213f, 0.715f - cos * 0.715f - sin * 0.715f, 0.072f - cos * 0.072f + sin * 0.928f, 0.0f, 0.0f,
                0.213f - cos * 0.213f + sin * 0.143f, 0.715f + cos * 0.285f + sin * 0.140f, 0.072f - cos * 0.072f - sin * 0.283f, 0.0f, 0.0f,
                0.213f - cos * 0.213f - sin * 0.787f, 0.715f - cos * 0.715f + sin * 0.715f, 0.072f + cos * 0.928f + sin * 0.072f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f, 0.0f
            };
            
            return SKColorFilter.CreateColorMatrix(mat);
        }

        public static SKColorFilter GetBrightnessColorMatrix(float value)
        {
            value /= 100;
            value = Math.Clamp(value, -1, 1);

            float[] mat =
            {
                1, 0, 0, 0, value,
                0, 1, 0, 0, value,
                0, 0, 1, 0, value,
                0, 0, 0, 1, 0
            };
            return SKColorFilter.CreateColorMatrix(mat);
        }

        public static SKColorFilter GetContrastColorMatrix(float value)
        {
            value /= 100;
            value = Math.Clamp(value, -1, 0.999f);
            return SKColorFilter.CreateHighContrast(false, SKHighContrastConfigInvertStyle.NoInvert, value);
        }

        public static SKColorFilter GetSaturationSaturationMatrix(float value)
        {
            value = Math.Clamp(value, -100, 100);

            float x = 1 + (value > 0 ? 3 * value / 100 : value / 100);
            float lumR = 0.3086f;
            float lumG = 0.6094f;
            float lumB = 0.0820f;

            float[] mat =
            {
                lumR * (1 - x) + x, lumG * (1 - x), lumB * (1 - x), 0, 0,
                lumR * (1 - x), lumG * (1 - x) + x, lumB * (1 - x), 0, 0,
                lumR * (1 - x), lumG * (1 - x), lumB * (1 - x) + x, 0, 0,
                0, 0, 0, 1, 0
            };

            return SKColorFilter.CreateColorMatrix(mat);
        }
    }
}