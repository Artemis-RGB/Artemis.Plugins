using System;
using System.Collections.Generic;
using System.Threading;
using OpenTK.Audio.OpenAL;
using ALAudioCapture = OpenTK.Audio.AudioCapture;

namespace Artemis.Plugins.Audio.LayerEffects.AudioCapture;

public class OpenAlAudioInput : IAudioInput
{
    #region Properties & Fields

    private ALAudioCapture _audioCapture;
    private Thread _captureThread;
    private short[] _buffer;

    public int BufferLength => SampleRate * 50;
    public int SampleRate => 44100;
    public float MasterVolume => 1;

    #endregion

    #region Event

    public event AudioData DataAvailable;

    #endregion

    #region Methods

    public void Initialize()
    {
        _buffer = new short[BufferLength];
        IList<string> recorders = ALAudioCapture.AvailableDevices;

        _audioCapture = new ALAudioCapture(recorders[0], SampleRate, ALFormat.Stereo16, BufferLength);
        _audioCapture.Start();
        
        _captureThread = new Thread(CaptureThread);
        _captureThread.Start();
    }

    private void CaptureThread()
    {
        try
        {
            while (_audioCapture.IsRunning)
            { 
                int samplesAvailable = _audioCapture.AvailableSamples;
                if (samplesAvailable <= 0)
                    continue;
                
                _audioCapture.ReadSamples(_buffer, samplesAvailable);

                short left = 0;
                for (int i = 0; i < samplesAvailable; i++)
                {
                    if (i % 2 == 0)
                    {
                        left = _buffer[i];
                    }
                    else
                    {
                        DataAvailable?.Invoke(left, _buffer[i]);
                    }
                }
            }
        }
        catch
        {
        }
    }

    public void Dispose()
    {
        _audioCapture.Stop();
        _audioCapture.Dispose();
        _audioCapture = null;
        _captureThread.Join();
        _captureThread = null;
    }

    #endregion
}