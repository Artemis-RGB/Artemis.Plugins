using System;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.Compression;

namespace Artemis.Plugins.LayerEffects.AudioVisualization.AudioCapture
{
    public class NAudioAudioInput : IAudioInput
    {
        #region Properties & Fields

        private MMDeviceEnumerator _deviceEnumerator;
        private MMDevice _endpoint;
        private WasapiLoopbackCapture _capture;

        public int SampleRate => _capture?.WaveFormat.SampleRate ?? -1;
        public float MasterVolume => _endpoint.AudioEndpointVolume.MasterVolumeLevelScalar * 100f;

        #endregion

        #region Event

        public event AudioData DataAvailable;

        #endregion

        #region Methods

        public void Initialize()
        {
            _deviceEnumerator = new MMDeviceEnumerator();
            _endpoint = _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            _capture = new WasapiLoopbackCapture();

            _capture.DataAvailable += (_, args) =>
            {
                WaveBuffer buffer = new WaveBuffer(args.Buffer) { ByteBufferCount = args.BytesRecorded };
                int count = buffer.FloatBufferCount;
                for (int i = 0; i < count; i += 2)
                    DataAvailable?.Invoke(buffer.FloatBuffer[i], buffer.FloatBuffer[i + 1]);
            };
            _capture.StartRecording();
        }

        public void Dispose()
        {
            _capture?.Dispose();
            _endpoint?.Dispose();
            _deviceEnumerator?.Dispose();

            _capture = null;
            _endpoint = null;
            _deviceEnumerator = null;
        }

        #endregion
    }
}