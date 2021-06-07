using Artemis.Plugins.Audio.Services;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using Serilog;

namespace Artemis.Plugins.Audio.LayerEffects.AudioCapture
{
    public class NAudioAudioInput : IAudioInput
    {
        #region Constructor

        public NAudioAudioInput(NAudioDeviceEnumerationService naudioDeviceEnumerationService, bool useCustomWasapiCapture, ILogger logger)
        {
            _naudioDeviceEnumerationService = naudioDeviceEnumerationService;
            _logger = logger;
            _useCustomWasapiCapture = useCustomWasapiCapture;
        }

        #endregion

        #region Event

        public event AudioData DataAvailable;

        #endregion

        #region Properties & Fields

        private readonly NAudioDeviceEnumerationService _naudioDeviceEnumerationService;
        private readonly ILogger _logger;
        private readonly bool _useCustomWasapiCapture;
        private MMDevice _endpoint;
        private WasapiCapture _capture;

        public int SampleRate => _capture?.WaveFormat.SampleRate ?? -1;
        public float MasterVolume => _endpoint.AudioEndpointVolume.MasterVolumeLevelScalar * 100f;

        #endregion

        #region Methods

        public void Initialize()
        {
            _endpoint = _naudioDeviceEnumerationService.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            
            // Don't initialize if there is no available device.
            if (_endpoint == null)
                return;

            _capture = _useCustomWasapiCapture
                ? CustomWasapiLoopbackCapture.CreateCustomWasapiLoopbackCapture(_endpoint, false, _logger)
                : new WasapiLoopbackCapture();
            _capture.RecordingStopped += CaptureOnRecordingStopped;

            if (_capture.WaveFormat.Channels != _endpoint.AudioClient.MixFormat.Channels)
            {
                // I want to log this to see how it behave in other setups. Don't know if this can happen, and if it happens, may could lead to exceptions
                _logger?.Verbose($"AudioEndPoint Waveformat has {_endpoint.AudioClient.MixFormat.Channels} channels but WasapiCapture was created for {_capture.WaveFormat.Channels} channels");
            }

            // Handle single-channel by passing the same data for left and right
            if (_capture.WaveFormat.Channels == 1)
                _capture.DataAvailable += ProcessMonoData;
            else if (_capture.WaveFormat.Channels == 4)
                _capture.DataAvailable += ProcessQuadraphonicData;
            // Handle 5.1 by averaging out the extra channels
            else if (_capture.WaveFormat.Channels == 6)
                _capture.DataAvailable += Process51Data;
            // Handle 7.1 by averaging out the extra channels
            else if (_capture.WaveFormat.Channels == 8)
                _capture.DataAvailable += Process71Data;
            // Anything else is limited to two channels
            else
                _capture.DataAvailable += ProcessStereoData;
            _capture.StartRecording();
        }

        private void ProcessMonoData(object sender, WaveInEventArgs e)
        {
            WaveBuffer buffer = new(e.Buffer) { ByteBufferCount = e.BytesRecorded };
            int count = buffer.FloatBufferCount;

            // Handle mono by passing the same data for left and right
            for (int i = 0; i < count; i++)
                DataAvailable?.Invoke(buffer.FloatBuffer[i], buffer.FloatBuffer[i]);
        }

        private void ProcessStereoData(object sender, WaveInEventArgs e)
        {
            WaveBuffer buffer = new(e.Buffer) { ByteBufferCount = e.BytesRecorded };
            int count = buffer.FloatBufferCount;

            for (int i = 0; i < count; i += _capture.WaveFormat.Channels)
                DataAvailable?.Invoke(buffer.FloatBuffer[i], buffer.FloatBuffer[i + 1]);
        }

        private void ProcessQuadraphonicData(object sender, WaveInEventArgs e)
        {
            WaveBuffer buffer = new(e.Buffer) { ByteBufferCount = e.BytesRecorded };
            int count = buffer.FloatBufferCount;

            for (int i = 0; i < count; i += 4)
            {
                DataAvailable?.Invoke(
                    // Front-left + back-left
                    (buffer.FloatBuffer[i] + buffer.FloatBuffer[i + 2]) / 2,
                    // Front-right + back-right 
                    (buffer.FloatBuffer[i + 1] + buffer.FloatBuffer[i + 3]) / 2
                );
            }
        }

        private void Process51Data(object sender, WaveInEventArgs e)
        {
            WaveBuffer buffer = new(e.Buffer) { ByteBufferCount = e.BytesRecorded };
            int count = buffer.FloatBufferCount;

            // Handle 5.1 by averaging out the extra channels
            for (int i = 0; i < count; i += 6)
            {
                DataAvailable?.Invoke(
                    // Front-left + center + base + back-left
                    (buffer.FloatBuffer[i] + buffer.FloatBuffer[i + 2] + buffer.FloatBuffer[i + 3] + buffer.FloatBuffer[i + 4]) / 4,
                    // Front-right + center + base + back-right 
                    (buffer.FloatBuffer[i + 1] + buffer.FloatBuffer[i + 2] + buffer.FloatBuffer[i + 3] + buffer.FloatBuffer[i + 5]) / 4
                );
            }
        }

        private void Process71Data(object sender, WaveInEventArgs e)
        {
            WaveBuffer buffer = new(e.Buffer) { ByteBufferCount = e.BytesRecorded };
            int count = buffer.FloatBufferCount;

            // Handle 7.1 by averaging out the extra channels
            for (int i = 0; i < count; i += 8)
            {
                DataAvailable?.Invoke(
                    // Front-left + center + base + back-left + mid-left
                    (buffer.FloatBuffer[i] + buffer.FloatBuffer[i + 2] + buffer.FloatBuffer[i + 3] + buffer.FloatBuffer[i + 4] + buffer.FloatBuffer[i + 6]) / 5,
                    // Front-right + center + base + back-right + mid-right
                    (buffer.FloatBuffer[i + 1] + buffer.FloatBuffer[i + 2] + buffer.FloatBuffer[i + 3] + buffer.FloatBuffer[i + 5] + buffer.FloatBuffer[i + 7]) / 5
                );
            }
        }

        private void CaptureOnRecordingStopped(object sender, StoppedEventArgs e)
        {
            // AUDCLNT_E_DEVICE_INVALIDATED
            // This means the device we're listening to somehow got invalidated (disconnected, modified, whatever)
            // Lets restart with a new capture
            if (e.Exception?.Message == "0x88890004")
            {
                Dispose();
                Initialize();
            }
            // Avoid Artemis Crash if plugin is disabled. COM things.
            else if (e.Exception == null) Dispose();
        }

        public void Dispose()
        {
            // Don't dispose the enumerator as it is part of a service and have to be alive
            // while the Plugin is enabled.
            _capture?.Dispose();
            _endpoint?.Dispose();

            _capture = null;
            _endpoint = null;
        }

        #endregion
    }
}