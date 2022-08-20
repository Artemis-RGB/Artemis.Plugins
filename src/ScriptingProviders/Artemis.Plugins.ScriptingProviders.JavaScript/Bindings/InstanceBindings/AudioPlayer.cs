using System;
using System.IO;
using System.Threading.Tasks;
using Artemis.Plugins.ScriptingProviders.JavaScript.Generators;
using Artemis.Plugins.ScriptingProviders.JavaScript.Jint;
using Jint;
using Jint.Runtime.Interop;
using ManagedBass;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Bindings.InstanceBindings
{
    public class AudioBinding : IInstanceBinding
    {
        public void Initialize(EngineManager engineManager)
        {
            Engine? engine = engineManager.Engine;
            if (engine == null)
                return;

            engine.Execute("const Audio = {}");
            engine.SetValue("GetAudio", (string p) =>
            {
                AudioPlayer audioPlayer = new(p);

                void OnEngineManagerOnDisposed(object? o, EventArgs eventArgs)
                {
                    audioPlayer.Dispose();
                    engineManager.Disposed -= OnEngineManagerOnDisposed;
                }

                engineManager.Disposed += OnEngineManagerOnDisposed;
                return audioPlayer;
            });
            engine.Execute("Audio.Create = GetAudio");
        }

        public string GetDeclaration()
        {
            return $@"
            declare class AudioFactory {{
                public Create(path: string): AudioPlayer; 
            }}

            const Audio = new AudioFactory();
            {new TypeScriptClass(null, typeof(AudioPlayer), true, TypeScriptClass.MaxDepth).GenerateCode("declare")}";
        }
    }

    public class AudioPlayer : IDisposable
    {
        private readonly string _path;
        private readonly MediaPlayer _player;
        private bool _loaded;
        private double _requestedVolume = 0.5;

        public AudioPlayer(string path)
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
            set
            {
                _player.Volume = value;
                _requestedVolume = value;
            }
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

                _player.Volume = _requestedVolume;
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