using System;
using System.Windows.Media;
using System.Windows.Threading;
using Jint.Native;
using Jint.Native.Function;
using Stylet;

namespace Artemis.Plugins.ScriptingProviders.JavaScript.Utilities
{
    public class Audio : IDisposable
    {
        private readonly string _path;
        private MediaPlayer _player;

        public Audio(string path)
        {
            _path = path;

            Execute.OnUIThreadSync(() =>
            {
                _player = new MediaPlayer();
                _player.MediaOpened += PlayerOnMediaOpened;
                _player.MediaFailed += PlayerOnMediaFailed;
                _player.MediaEnded += PlayerOnMediaEnded;
                _player.Open(new Uri(_path));
            });
        }

        public double duration
        {
            get
            {
                double value = 0;
                Execute.OnUIThreadSync(() => value = _player.NaturalDuration.TimeSpan.TotalSeconds);
                return value;
            }
        }

        public bool ended
        {
            get
            {
                bool value = false;
                Execute.OnUIThreadSync(() => value = _player.Position >= _player.NaturalDuration.TimeSpan);
                return value;
            }
        }

        public double volume
        {
            get
            {
                double value = 0;
                Execute.OnUIThreadSync(() => value = _player.Volume);
                return value;
            }
            set => Execute.OnUIThreadSync(() => _player.Volume = value);
        }

        public bool loop { get; set; }

        public void play()
        {
            Execute.OnUIThreadSync(() => _player.Play());
        }

        public void pause()
        {
            Execute.OnUIThreadSync(() => _player.Pause());
        }

        public void stop()
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
            if (loop)
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