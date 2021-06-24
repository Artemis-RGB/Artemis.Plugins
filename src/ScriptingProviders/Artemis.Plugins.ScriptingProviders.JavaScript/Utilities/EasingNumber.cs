using System;
using Artemis.Core;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Utilities
{
    public class EasingNumber
    {
        private DateTime _lastUpdate;
        private double _progress;
        private double _progressNormalized;

        public EasingNumber(double start, double end, int delay, Easings.Functions easing)
        {
            Start = start;
            End = end;
            Delay = delay;
            Easing = easing;
        }

        public double Start { get; set; }
        public double End { get; set; }
        public int Delay { get; set; }
        public Easings.Functions Easing { get; set; }

        public double Current
        {
            get
            {
                Update();
                return Start + (End - Start) * Easings.Interpolate(_progressNormalized, Easing);
            }
        }

        public bool IsFinished
        {
            get
            {
                Update();
                return Math.Abs(Current - End) < 0.001;
            }
        }

        public void followUp(double end)
        {
            Start = End;
            End = end;

            reset();
        }

        public void reset()
        {
            _progress = 0;
            _progressNormalized = 0;
        }

        private void Update()
        {
            if (_progressNormalized >= 1)
                return;

            _progress += (DateTime.Now - _lastUpdate).TotalMilliseconds;
            _progressNormalized = Math.Clamp(_progress / Delay, 0, 1);

            _lastUpdate = DateTime.Now;
        }
    }
}