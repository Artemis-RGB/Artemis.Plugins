using Artemis.Core;
using RGB.NET.Core;

namespace Artemis.Plugins.Devices.DMX.Settings
{
    public class LedDefinition : CorePropertyChanged
    {
        private int _b;
        private int _g;
        private int _r;
        
        public int R
        {
            get => _r;
            set => SetAndNotify(ref _r, value);
        }

        public int G
        {
            get => _g;
            set => SetAndNotify(ref _g, value);
        }

        public int B
        {
            get => _b;
            set => SetAndNotify(ref _b, value);
        }
    }
}