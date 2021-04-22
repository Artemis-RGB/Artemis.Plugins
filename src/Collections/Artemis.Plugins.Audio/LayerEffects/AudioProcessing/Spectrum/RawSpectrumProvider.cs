using Artemis.Plugins.LayerEffects.AudioVisualization.Helper;

namespace Artemis.Plugins.LayerEffects.AudioVisualization.AudioProcessing.Spectrum
{
    public class RawSpectrumProvider : AbstractSpectrum
    {
        #region Constructors

        public RawSpectrumProvider(float[] data)
        {
            int dataReferenceCount = (data.Length - 1) * 2;

            Bands = new Band[data.Length];

            for (int i = 0; i < Bands.Length; i++)
                Bands[i] = new Band(FrequencyHelper.GetFrequencyOfIndex(i, dataReferenceCount), FrequencyHelper.GetFrequencyOfIndex(i, dataReferenceCount), new[] { data[i] });
        }

        #endregion
    }
}
