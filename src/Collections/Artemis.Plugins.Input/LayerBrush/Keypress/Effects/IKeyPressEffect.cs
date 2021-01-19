using Artemis.Core;
using SkiaSharp;

namespace Artemis.Plugins.Input.LayerBrush.Keypress.Effects
{
    public interface IKeyPressEffect
    {
        ArtemisLed Led { get; }
        SKPoint Position { get; set; }
        
        bool Finished { get; }
        bool AllowDuplicates { get; }
        void Update(double deltaTime);
        void Render(SKCanvas canvas);
        void Respawn();
        void Despawn();
    }
}