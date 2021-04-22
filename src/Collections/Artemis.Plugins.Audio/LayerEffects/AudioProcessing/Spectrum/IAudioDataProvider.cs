using System;

namespace Artemis.Plugins.LayerEffects.AudioVisualization.AudioProcessing.Spectrum
{
    public interface IAudioDataProvider
    {
        public int BufferSize { get; }

        void GetDataInto(in Span<float> data);
    }
}
