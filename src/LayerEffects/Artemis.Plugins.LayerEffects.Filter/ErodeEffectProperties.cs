﻿using Artemis.Core;

namespace Artemis.Plugins.LayerEffects.Filter
{
    public class ErodeEffectProperties : LayerEffectPropertyGroup
    {
        [PropertyDescription(Description = "The amount of erode to apply", MinInputValue = 0)]
        public SKSizeLayerProperty ErodeRadius { get; set; }

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