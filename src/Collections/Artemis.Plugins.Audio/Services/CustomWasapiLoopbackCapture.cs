using NAudio.CoreAudioApi;
using Serilog;
using System;
using System.Threading;

namespace Artemis.Plugins.Audio.Services
{
    public sealed class CustomWasapiLoopbackCapture : WasapiCapture
    {
        #region Constructor (Private)

        // Not intended for direct use because we know it may fail if using audio engines like Nahimic
        private CustomWasapiLoopbackCapture(MMDevice mDevice, bool useEventSync, int channels)
            : base(mDevice, useEventSync, 100)
        {

            try
            {
                // NAudio won't allow set custom channel count for the WasapiLoopCapture and will fail if we set a new MixFormat. NAudio bug.
                var reflectedChannels = this.WaveFormat.GetType().GetField("channels", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                reflectedChannels.SetValue(this.WaveFormat, (Int16)channels);
            }
            catch
            {
                // TODO: Log
            }
        }

        #endregion

        #region Methods


        protected override AudioClientStreamFlags GetAudioClientStreamFlags()
        {
            return AudioClientStreamFlags.Loopback;
        }

        #endregion

        #region Static Methods (Factory)

        public static CustomWasapiLoopbackCapture CreateCustomWasapiLoopbackCapture(MMDevice mDevice, bool UseWasapiEventSync, ILogger logger = null)
        {
            int channels = mDevice.AudioClient.MixFormat.Channels;
            CustomWasapiLoopbackCapture audioCapturer = null;
            for (int i = channels; (i > 0 && audioCapturer == null); i--)
            {
                try
                {
                    audioCapturer = new CustomWasapiLoopbackCapture(mDevice, UseWasapiEventSync, i);

                    // We may crash here based on the channel count.
                    audioCapturer.StartRecording();
                    audioCapturer.StopRecording();// This is not instant. 
                    while (audioCapturer.CaptureState != CaptureState.Stopped) Thread.Sleep(100); // Find a better way to stop and wait.

                    logger?.Information($"WasapiCapture succesfully created for {i} channels with {audioCapturer.WaveFormat.SampleRate}Hz and {audioCapturer.WaveFormat.BitsPerSample}Bps");

                    break; // succeeded.
                }
                catch
                {
                    logger?.Warning($"WasapiCapture creation failed with {i} channels. Trying with one less channel");
                    audioCapturer = null;
                }
            }

            if (channels == 0)
            {
                logger?.Error($"WasapiCapture cannot be created. No valid channel config found");
            }

            return audioCapturer;
        }

        #endregion
    }
}
