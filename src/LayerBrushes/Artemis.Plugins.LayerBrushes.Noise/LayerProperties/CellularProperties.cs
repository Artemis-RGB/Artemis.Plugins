using Artemis.Core;
using static FastNoiseLite;

namespace Artemis.Plugins.LayerBrushes.Noise
{
    public class CellularProperties : LayerPropertyGroup
    {
        [PropertyDescription]
        public EnumLayerProperty<CellularDistanceFunction> DistanceFunction { get; set; }

        [PropertyDescription]
        public EnumLayerProperty<CellularReturnType> ReturnType { get; set; }

        [PropertyDescription(MinInputValue = -1.2f, MaxInputValue =1.2f)]
        public FloatLayerProperty Jitter { get; set; }

        protected override void DisableProperties()
        {
        }

        protected override void EnableProperties()
        {            
        }

        protected override void PopulateDefaults()
        {
            DistanceFunction.DefaultValue = CellularDistanceFunction.EuclideanSq;
            ReturnType.DefaultValue = CellularReturnType.CellValue;
            Jitter.DefaultValue = 1f;
        }
    }
}