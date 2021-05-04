using NAudio.CoreAudioApi;
using Serilog;
using System;
using System.Reflection;
using System.Threading;

namespace Artemis.Plugins.Audio.Services
{
    public sealed class CustomWasapiLoopbackCapture : WasapiCapture
    {

        #region Properties & Fields
        protected override AudioClientStreamFlags GetAudioClientStreamFlags() => AudioClientStreamFlags.Loopback;

        #endregion

        #region Constructor (Private)

        // Not intended for direct use because we know it may fail if using audio engines like Nahimic
        private CustomWasapiLoopbackCapture(MMDevice mDevice, bool useEventSync, int channels) : base(mDevice, useEventSync, 100)
        {
            try
            {
                // NAudio won't allow set custom channel count for the WasapiLoopCapture and will fail if we set a new MixFormat. NAudio bug. 
                // In Fact something like this.WaveFormat = this.Waveformat is enough to make it fail

                FieldInfo reflectedChannelsField = this.WaveFormat.GetType().GetField("channels", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                reflectedChannelsField.SetValue(this.WaveFormat, (Int16)channels);
            }
            catch
            {
                /* TODO: Log. Will break if Naudio internal changes. Not highly probably */
                throw;
            }
        }

        #endregion

        #region Methods (Static)

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

                    // This is not instant.
                    audioCapturer.StopRecording();
                    while (audioCapturer.CaptureState != CaptureState.Stopped) Thread.Sleep(50); // Find a better way to stop and wait.

                    logger?.Verbose($"CustomWasapiCapture succesfully created for {i} channels with {audioCapturer.WaveFormat.SampleRate}Hz and {audioCapturer.WaveFormat.BitsPerSample}bps");

                    break; // succeeded.
                }
                catch (Exception e)
                {
                    logger?.Warning($"CustomWasapiCapture creation failed for {i} channels with {audioCapturer.WaveFormat.SampleRate}Hz and {audioCapturer.WaveFormat.BitsPerSample}bps.\r\nException:{e}\r\nTrying with one less channel");
                    audioCapturer = null;
                }
            }

            if (channels == 0) logger?.Error($"CustomWasapiCapture cannot be created. No valid channel config found. Try disabling third party Audio Engines");

            return audioCapturer;
        }

        #endregion
    }
}
