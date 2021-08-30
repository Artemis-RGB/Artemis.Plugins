using System;
using Artemis.Core;
using Artemis.Plugins.ScriptingProviders.JavaScript.Generators;
using Jint;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Bindings.InstanceBindings
{
    public class EasingNumberBinding : IInstanceBinding
    {
        public void Initialize(Engine engine)
        {
            TypeScriptEnum typeScriptEnum = new(typeof(Easings.Functions));
            string enums = "";
            for (int index = 0; index < typeScriptEnum.Names.Length; index++)
                enums = enums + typeScriptEnum.Names[index] + ": " + typeScriptEnum.Values[index] + ",\r\n";

            string enumDeclaration = "if (!Artemis.Core) {\r\n" +
                                     "  Artemis.Core = {}\r\n" +
                                     "}\r\n" +
                                     "Artemis.Core.EasingsFunctions = {\r\n" +
                                     $"   {enums.Trim()}\r\n" +
                                     "}";

            engine.Execute(enumDeclaration);
        }

        public string? GetDeclaration()
        {
            return new TypeScriptClass(null, typeof(TimeSpan), true, TypeScriptClass.MaxDepth).GenerateCode("declare");
        }
    }

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

        public void FollowUp(double end)
        {
            Start = End;
            End = end;

            Reset();
        }

        public void Reset()
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