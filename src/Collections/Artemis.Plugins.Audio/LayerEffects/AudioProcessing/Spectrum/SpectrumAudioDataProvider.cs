using System;
using Artemis.Plugins.Audio.LayerEffects.AudioCapture;

namespace Artemis.Plugins.Audio.LayerEffects.AudioProcessing.Spectrum
{
    internal class SpectrumAudioDataProvider : IAudioDataProvider
    {
        #region Properties & Fields

        private readonly AudioBuffer _audioBuffer;
        private readonly Channel _channel;

        public int BufferSize => _audioBuffer.Size;

        #endregion

        #region Constructors

        public SpectrumAudioDataProvider(AudioBuffer audioBuffer, Channel channel)
        {
            this._audioBuffer = audioBuffer;
            this._channel = channel;
        }

        #endregion

        #region Methods

        public void GetDataInto(in Span<float> data)
        {
            switch (_channel)
            {
                case Channel.Mix:
                    _audioBuffer.CopyMixInto(data, 0);
                    break;

                case Channel.Left:
                    _audioBuffer.CopyLeftInto(data, 0);
                    break;

                case Channel.Right:
                    _audioBuffer.CopyRightInto(data, 0);
                    break;
            }
        }

        #endregion
    }
}
