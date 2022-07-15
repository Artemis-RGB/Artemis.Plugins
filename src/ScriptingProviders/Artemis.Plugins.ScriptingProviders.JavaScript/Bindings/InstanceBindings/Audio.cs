using System;
using System.IO;
using System.Windows.Media;
using Artemis.Plugins.ScriptingProviders.JavaScript.Generators;
using Artemis.Plugins.ScriptingProviders.JavaScript.Jint;
using Jint;
using Jint.Runtime.Interop;
using Stylet;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Bindings.InstanceBindings
{
    public class AudioBinding : IInstanceBinding
    {
        public void Initialize(EngineManager engineManager)
        {
            engineManager.Engine.SetValue("Audio", TypeReference.CreateTypeReference(engineManager.Engine, typeof(Audio)));
        }

        public string GetDeclaration()
        {
            return new TypeScriptClass(null, typeof(Audio), true, TypeScriptClass.MaxDepth).GenerateCode("declare");
        }
    }

    public class Audio : IDisposable
    {
        private MediaPlayer _player;

        public Audio(string path)
        {
            if (!File.Exists(path))
                throw new Exception($"File '{path}' not found.");

            _player = null!;
            Execute.OnUIThreadSync(() =>
            {
                _player = new MediaPlayer();
                _player.MediaOpened += PlayerOnMediaOpened;
                _player.MediaFailed += PlayerOnMediaFailed;
                _player.MediaEnded += PlayerOnMediaEnded;
                _player.Open(new Uri(path));
            });
        }

        public double Duration
        {
            get
            {
                double value = 0;
                Execute.OnUIThreadSync(() => value = _player.NaturalDuration.TimeSpan.TotalSeconds);
                return value;
            }
        }

        public bool Ended
        {
            get
            {
                bool value = false;
                Execute.OnUIThreadSync(() => value = _player.Position >= _player.NaturalDuration.TimeSpan);
                return value;
            }
        }

        public double Volume
        {
            get
            {
                double value = 0;
                Execute.OnUIThreadSync(() => value = _player.Volume);
                return value;
            }
            set => Execute.OnUIThreadSync(() => _player.Volume = value);
        }

        public bool Loop { get; set; }

        public void Play()
        {
            Execute.OnUIThreadSync(() => _player.Play());
        }

        public void Pause()
        {
            Execute.OnUIThreadSync(() => _player.Pause());
        }

        public void Stop()
        {
            Execute.OnUIThreadSync(() => _player.Stop());
        }

        private void PlayerOnMediaOpened(object? sender, EventArgs e)
        {
            if (_player.HasVideo)
            {
                _player.Close();
                throw new Exception("Only audio is supported");
            }
        }

        private void PlayerOnMediaFailed(object? sender, ExceptionEventArgs e)
        {
        }

        private void PlayerOnMediaEnded(object? sender, EventArgs e)
        {
            if (Loop)
            {
                _player.Position = TimeSpan.Zero;
                _player.Play();
            }
        }

        public void Dispose()
        {
            _player.MediaOpened -= PlayerOnMediaOpened;
            _player.MediaFailed -= PlayerOnMediaFailed;
            _player.MediaEnded -= PlayerOnMediaEnded;

            _player.Stop();
            _player.Close();
        }
    }
}