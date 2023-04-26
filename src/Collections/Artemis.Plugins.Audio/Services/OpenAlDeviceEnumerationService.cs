using System;
using Artemis.Plugins.Audio.LayerEffects.AudioCapture;
using OpenTK.Audio;

namespace Artemis.Plugins.Audio.Services;

public class OpenAlDeviceEnumerationService : IAudioEnumerationService
{
    public event EventHandler DefaultDeviceChanged;
    
    public string GetDefaultAudioEndpoint()
    {
        return AudioCapture.DefaultDevice;
    }

    public IAudioInput CreateDefaultAudioInput(bool compatibilityMode)
    {
        return new OpenAlAudioInput();
    }

    public void Dispose()
    {

    }
}