using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis.Core;

namespace Artemis.Plugins.LayerBrushes.Color.PropertyGroups
{
   public class SweepGradientBrushProperties : LayerPropertyGroup
    {
        [PropertyDescription(Description = "The gradient of the brush")]
        public ColorGradientLayerProperty Colors { get; set; }

        [PropertyDescription(Name = "Colors multiplier", Description = "How many times to repeat the colors in the selected gradient", DisableKeyframes = true, MinInputValue = 0, MaxInputValue = 10)]
        public IntLayerProperty ColorsMultiplier { get; set; }

        [PropertyDescription(Description = "Change the rotation of the gradient without affecting the rotation of the shape", InputAffix = "°")]
        public FloatLayerProperty Rotation { get; set; }

        #region Overrides of LayerPropertyGroup

        protected override void PopulateDefaults()
        {
            Colors.DefaultValue = ColorGradient.GetUnicornBarf();
        }

        protected override void EnableProperties()
        {
        }

        protected override void DisableProperties()
        {
        }

        #endregion
    }
}
