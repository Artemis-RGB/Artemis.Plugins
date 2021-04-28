using System.ComponentModel;
using Artemis.Core;

namespace Artemis.Plugins.Audio.LayerEffects.AudioVisualization
{
    public enum ValueMode
    {
        [Description("Sum")] Sum = 1,
        [Description("Average")] Average = 2,
        [Description("Max")] Max = 3
    }

    public enum SpectrumMode
    {
        [Description("Logarithmic")] Logarithmic = 1,
        [Description("Gamma")] Gamma = 2,
        [Description("Linear")] Linear = 3
    }

    public enum Channel
    {
        [Description("Mix")] Mix = 0,
        [Description("Left")] Left = 1,
        [Description("Right")] Right = 2
    }

    public class AudioVisualizationEffectProperties : LayerPropertyGroup
    {
        #region Properties & Fields

        [PropertyDescription(Description = "The channel to listen to")]
        public EnumLayerProperty<Channel> Channel { get; set; }

        [PropertyDescription(Description = "The method to calculate the value (Sum, Average or Max)")]
        public EnumLayerProperty<ValueMode> ValueMode { get; set; }

        [PropertyDescription(Description = "The algorithm the spectrum is calculated with (Logarithmic, Gamma or Linear")]
        public EnumLayerProperty<SpectrumMode> SpectrumMode { get; set; }

        [PropertyDescription(Description = "The number of bars to display", MinInputValue = 1, MaxInputValue = 96)]
        public IntLayerProperty Bars { get; set; }

        [PropertyDescription(Description = "The amount of smoothing", MinInputValue = 1, MaxInputValue = 10)]
        public FloatLayerProperty Smoothing { get; set; }

        [PropertyDescription(Description = "The minimum frequency", MinInputValue = 0, MaxInputValue = 22100)]
        public FloatLayerProperty MinFrequency { get; set; }

        [PropertyDescription(Description = "The maximum frequency", MinInputValue = 0, MaxInputValue = 22100)]
        public FloatLayerProperty MaxFrequency { get; set; }

        [PropertyDescription(Description = "The reference-level", MinInputValue = 1, MaxInputValue = 240)]
        public FloatLayerProperty ReferenceLevel { get; set; }

        [PropertyDescription(Description = "The amount of peak-emphasization", MinInputValue = 0, MaxInputValue = 2)]
        public FloatLayerProperty EmphasisePeaks { get; set; }

        [PropertyDescription(Description = "The gamma-value used when spectrum-mode is gamma", MinInputValue = 1, MaxInputValue = 6)]
        public IntLayerProperty Gamma { get; set; }

        #endregion

        #region Methods

        protected override void PopulateDefaults()
        {
            Channel.DefaultValue = AudioVisualization.Channel.Mix;
            ValueMode.DefaultValue = AudioVisualization.ValueMode.Sum;
            SpectrumMode.DefaultValue = AudioVisualization.SpectrumMode.Logarithmic;
            Bars.DefaultValue = 48;
            Smoothing.DefaultValue = 3;
            MinFrequency.DefaultValue = 60;
            MaxFrequency.DefaultValue = 15000;
            ReferenceLevel.DefaultValue = 90;
            EmphasisePeaks.DefaultValue = 0.5f;
            Gamma.DefaultValue = 2;
        }

        protected override void EnableProperties()
        { }

        protected override void DisableProperties()
        { }

        #endregion
    }
}