﻿using Artemis.Core;

namespace Artemis.Plugins.LayerEffects.Filter
{
    public class DilateEffectProperties : LayerEffectPropertyGroup
    {
        [PropertyDescription(Description = "The amount of dilation to apply", MinInputValue = 0)]
        public SKSizeLayerProperty DilateRadius { get; set; }

        protected override void PopulateDefaults()
        {
        }

        protected override void EnableProperties()
        {
        }

        protected override void DisableProperties()
        {
        }
    }
}