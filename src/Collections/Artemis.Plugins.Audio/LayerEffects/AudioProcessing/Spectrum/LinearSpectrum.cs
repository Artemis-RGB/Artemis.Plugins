using System;
using Artemis.Plugins.Audio.LayerEffects.AudioVisualization.Extensions;
using Artemis.Plugins.Audio.LayerEffects.AudioVisualization.Helper;

namespace Artemis.Plugins.Audio.LayerEffects.AudioVisualization.AudioProcessing.Spectrum
{
    public class LinearSpectrum : AbstractSpectrum
    {
        #region Constructors

        public LinearSpectrum(float[] data, int bands, float minFrequency = -1, float maxFrequency = -1)
        {
            int dataReferenceCount = (data.Length - 1) * 2;

            int fromIndex = minFrequency < 0 ? 0 : FrequencyHelper.GetIndexOfFrequency(minFrequency, dataReferenceCount).Clamp(0, data.Length - 1 - bands); // -bands since we need at least enough data to get our bands
            int toIndex = maxFrequency < 0 ? data.Length - 1 : FrequencyHelper.GetIndexOfFrequency(maxFrequency, dataReferenceCount).Clamp(fromIndex, data.Length - 1);

            int usableSourceData = Math.Max(bands, (toIndex - fromIndex) + 1);

            Bands = new Band[bands];

            double frequenciesPerBand = (double)usableSourceData / bands;
            double frequencyCounter = 0;

            int index = fromIndex;
            for (int i = 0; i < Bands.Length; i++)
            {
                frequencyCounter += frequenciesPerBand;
                int count = (int)frequencyCounter;

                float[] bandData = new float[count];
                Array.Copy(data, index, bandData, 0, count);
                Bands[i] = new Band(FrequencyHelper.GetFrequencyOfIndex(index, dataReferenceCount),
                                    FrequencyHelper.GetFrequencyOfIndex(index + count, dataReferenceCount),
                                    bandData);

                index += count;
                frequencyCounter -= count;
            }
        }

        #endregion
    }
}
