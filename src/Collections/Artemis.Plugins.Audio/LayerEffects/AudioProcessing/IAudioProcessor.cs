using System;

namespace Artemis.Plugins.Audio.LayerEffects.AudioVisualization.AudioProcessing
{
    public interface IAudioProcessor : IDisposable
    {
        bool IsActive { get; set; }

        void Initialize();
        void Update();
    }
}
