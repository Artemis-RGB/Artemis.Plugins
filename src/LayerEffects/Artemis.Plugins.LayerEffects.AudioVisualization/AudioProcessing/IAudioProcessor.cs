using System;

namespace Artemis.Plugins.LayerEffects.AudioVisualization.AudioProcessing
{
    public interface IAudioProcessor : IDisposable
    {
        bool IsActive { get; set; }

        void Initialize();
        void Update();
    }
}
