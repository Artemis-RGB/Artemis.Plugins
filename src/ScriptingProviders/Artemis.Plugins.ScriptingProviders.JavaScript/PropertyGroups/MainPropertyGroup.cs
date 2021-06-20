using Artemis.Core;
using System.ComponentModel;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.PropertyGroups
{
    public class MainPropertyGroup : LayerPropertyGroup
    {
        [PropertyDescription]
        public FloatLayerProperty SampleFloatProperty { get; set; }

        // Using the PropertyDescription attribute we can provide the UI with extra information
        [PropertyDescription(Description = "This is a percentage", InputAffix = "%", MinInputValue = 0, MaxInputValue = 100)]
        public FloatLayerProperty SamplePercentageProperty { get; set; }

        [PropertyDescription(Name = "Cool enum property")]
        public EnumLayerProperty<SampleEnum> EnumLayerProperty { get; set; }

        [PropertyDescription(Description = "This is a size property")]
        public SKSizeLayerProperty SampleSizeProperty { get; set; }

        protected override void PopulateDefaults()
        {
            // This is where you should set default values for your properties
            SampleFloatProperty.DefaultValue = 10f;
            SamplePercentageProperty.DefaultValue = 100f;
            EnumLayerProperty.DefaultValue = SampleEnum.EnumValue2;

            // However because this is optional, we'll skip giving SampleSizeProperty a default value
        }

        protected override void EnableProperties()
        {
            // This is where you do any sort of initialization on your property group
        }

        protected override void DisableProperties()
        {
            // If you subscribed to events or need to clean up, do it here
        }
    }

    public enum SampleEnum
    {
        [Description("First enum value")]
        EnumValue1,

        [Description("Second enum value")]
        EnumValue2
    }
}