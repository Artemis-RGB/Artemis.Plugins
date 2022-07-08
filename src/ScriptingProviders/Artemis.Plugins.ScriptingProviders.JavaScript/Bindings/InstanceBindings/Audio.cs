using System;
using System.IO;
using System.Threading.Tasks;
using Artemis.Plugins.ScriptingProviders.JavaScript.Generators;
using Jint;
using Jint.Runtime.Interop;
using ManagedBass;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Bindings.InstanceBindings
{
    public class AudioBinding : IInstanceBinding
    {
        public void Initialize(Engine engine)
        {
            engine.SetValue("Audio", TypeReference.CreateTypeReference(engine, typeof(Audio)));
        }

        public string GetDeclaration()
        {
            return new TypeScriptClass(null, typeof(Audio), true, TypeScriptClass.MaxDepth).GenerateCode("declare");
        }
    }

    public class Audio : IDisposable
    {
        private readonly string _path;
        private readonly MediaPlayer _player;
        private bool _loaded;

        public Audio(string path)
        {
            if (!File.Exists(path))
                throw new Exception($"File '{path}' not found.");

            _path = path;
            _player = new MediaPlayer();
        }

        public double Duration => _player.Duration.TotalSeconds;

        public bool Ended => _player.Position >= _player.Duration;

        public double Volume
        {
            get => _player.Volume;
            set => _player.Volume = value;
        }

        public bool Loop
        {
            get => _player.Loop;
            set => _player.Loop = value;
        }

        public void Play()
        {
            Task.Run(async () =>
            {
                if (!_loaded)
                {
                    await _player.LoadAsync(_path);
                    _loaded = true;
                }

                _player.Loop = Loop;
                _player.Play();
            });
        }

        public void Pause()
        {
            _player.Pause();
        }

        public void Stop()
        {
            _player.Stop();
        }

        public void Dispose()
        {
            _player.Stop();
            _player.Dispose();
        }
    }
}