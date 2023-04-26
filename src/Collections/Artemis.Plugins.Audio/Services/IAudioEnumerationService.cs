using System;
using Artemis.Core.Services;
using Artemis.Plugins.Audio.LayerEffects.AudioCapture;

namespace Artemis.Plugins.Audio.Services;

public interface IAudioEnumerationService :  IDisposable
{
    public event EventHandler DefaultDeviceChanged;

    IAudioInput CreateDefaultAudioInput(bool compatibilityMode);
}