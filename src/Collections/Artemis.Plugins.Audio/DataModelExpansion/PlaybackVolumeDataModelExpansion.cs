using Artemis.Core.DataModelExpansions;
using Artemis.Plugins.LayerEffects.AudioVisualization.Services;
using NAudio.CoreAudioApi;
using Serilog;
using System;

namespace Artemis.Plugins.DataModelExpansions.PlaybackVolume
{
    public class PlaybackVolumeDataModelExpansion : DataModelExpansion<PlaybackVolumeDataModel>
    {
        #region Properties & Fields

        private readonly NAudioDeviceEnumerationService _naudioDeviceEnumerationService;
        private readonly ILogger _logger;
        private readonly object _audioEventLock = new object();
        private bool _playbackDeviceChanged = false;
        private MMDevice _playbackDevice;
        private AudioEndpointVolume _audioEndpointVolume;

        #endregion

        #region Constructor
        public PlaybackVolumeDataModelExpansion(ILogger logger, NAudioDeviceEnumerationService naudioDeviceEnumerationService)
        {
            _logger = logger;
            _naudioDeviceEnumerationService = naudioDeviceEnumerationService;
        }

        #endregion

        #region Plugin Methods

        public override void Enable()
        {
            _naudioDeviceEnumerationService.NotificationClient.DefaultDeviceChanged += NotificationClient_DefaultDeviceChanged;
            UpdatePlaybackDevice(true);
            AddTimedUpdate(TimeSpan.FromMilliseconds(10), UpdatePeakVolume);
        }

        public override void Disable()
        {
            _naudioDeviceEnumerationService.NotificationClient.DefaultDeviceChanged -= NotificationClient_DefaultDeviceChanged;
            _audioEndpointVolume.Dispose();
            _audioEndpointVolume = null;
            FreePlaybackDevice();
        }

        public override void Update(double deltaTime)
        {
            if (_playbackDeviceChanged)
            {
                UpdatePlaybackDevice();
            }
        }

        #endregion

        #region Update DataModel Methods

        private void UpdatePeakVolume(double deltaTime)
        {
            if (this.IsEnabled == false)
            {
                // To avoid null object exception on _enumerator use after plugin is disabled.
                return;
            }

            if (_playbackDevice == null)
            {
                // To avoid null object exception on device change
                return;
            }

            // Update Main Voulume Peak
            lock (_audioEventLock) // To avoid query an Device/EndPoint that is not the current device anymore or has more or less channels
            {
                // Absolute master peak volume 
                float _peakVolumeNormalized = _playbackDevice.AudioMeterInformation.MasterPeakValue;
                DataModel.PeakVolumeNormalized = _peakVolumeNormalized;
                DataModel.PeakVolume = _peakVolumeNormalized * 100f;

                // Master peak volume relative to master volume
                DataModel.PeakVolumeRelativeNormalized = _peakVolumeNormalized * DataModel.VolumeNormalized;
                DataModel.PeakVolumeRelative = _peakVolumeNormalized * 100f * DataModel.VolumeNormalized;

                // Update Channels Peak
                var channelsVolumeNormalized = _playbackDevice.AudioMeterInformation.PeakValues;
                for (int i = 0; i < DataModel.Channels.DynamicChildren.Count; i++)
                {
                    var channelDataModel = DataModel.Channels.GetDynamicChild<ChannelDataModel>(string.Format("Channel {0}", i));
                    channelDataModel.Value.PeakVolumeNormalized = Math.Max(channelsVolumeNormalized[i], 0);
                    channelDataModel.Value.PeakVolume = Math.Max(channelsVolumeNormalized[i] * 100f, 0);
                }

                if (_peakVolumeNormalized > 0)
                {
                    DataModel.SoundPlayed.Trigger();
                }
            }
        }

        private void UpdateVolumeDataModel()
        {
            DataModel.VolumeChanged.Trigger();
            DataModel.VolumeNormalized = (_audioEndpointVolume.MasterVolumeLevelScalar);
            DataModel.Volume = DataModel.VolumeNormalized * 100f;
            DataModel.ChannelCount = _audioEndpointVolume.Channels.Count;
            DataModel.DeviceState = _playbackDevice.State;
            DataModel.Muted = _audioEndpointVolume.Mute;

            lock (_audioEventLock)
            {
                for (int i = 0; i < _audioEndpointVolume.Channels.Count; i++)
                {
                    var channelDataModel = DataModel.Channels.GetDynamicChild<ChannelDataModel>(string.Format("Channel {0}", i));
                    float volumeNormalized = _audioEndpointVolume.Channels[i].VolumeLevelScalar;
                    channelDataModel.Value.VolumeNormalized = volumeNormalized;
                    channelDataModel.Value.Volume = volumeNormalized * 100f;
                }
            }
        }

        private void PopulateChannels()
        {
            DataModel.Channels.ClearDynamicChildren();
            _logger.Information(string.Format("Playback device {0} channel list cleared", _playbackDevice.FriendlyName));
            _logger.Information(string.Format("Preparing to populate {0} channels for device {1}", _audioEndpointVolume.Channels.Count, _playbackDevice.FriendlyName));
            for (int i = 0; i < _audioEndpointVolume.Channels.Count; i++)
            {
                DataModel.Channels.AddDynamicChild(
                    string.Format("Channel {0}", i),
                    new ChannelDataModel()
                    {
                        ChannelIndex = i
                    },
                    string.Format("Channel {0}", i)
                    );
                _logger.Information(string.Format("Playback device {0} channel {1} populated", _playbackDevice.FriendlyName, i));
            }
        }


        #endregion

        #region Audio Management methods

        private void NotificationClient_DefaultDeviceChanged()
        {
            _playbackDeviceChanged = true;
            // Workarround. MMDevice won't dispose if Dispose() is called from 
            // non parent thread and NaudioNotificationClient callbacks come from another thread.
            // We will use Update() mrhod to dispose MMDevice from creator thread because this (NotificationClient_DefaultDeviceChanged()) is called from another thread
        }

        private void UpdatePlaybackDevice(bool firstRun = false)
        {
            lock (_audioEventLock)
            {
                if (!firstRun) { FreePlaybackDevice(); };
                SetPlaybackDevice();
                PopulateChannels();
                _playbackDeviceChanged = false;
                UpdateVolumeDataModel();
            }
        }

        private void FreePlaybackDevice()
        {
            string disposingPlaybackDeviceFriendlyName = _playbackDevice?.FriendlyName ?? "Unknown";
            _audioEndpointVolume?.Dispose();
            _playbackDevice?.Dispose();
            _playbackDevice = null;
            _logger.Information(string.Format("Playback device {0} unregistered as source device to fill Playback volume data model", disposingPlaybackDeviceFriendlyName));
        }

        private void SetPlaybackDevice()
        {
            _playbackDevice = _naudioDeviceEnumerationService.Enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);

            if (_playbackDevice == null)
            {
                _logger.Warning("No audio device found with Console role");
                return;
            }

            _audioEndpointVolume = _playbackDevice.AudioEndpointVolume;

            _audioEndpointVolume.OnVolumeNotification += _audioEndpointVolume_OnVolumeNotification;
            DataModel.DefaultDeviceName = _playbackDevice.FriendlyName;

            _logger.Information(string.Format("Playback device {0} registered to to fill Playback volume data model", _playbackDevice.FriendlyName));
        }

        private void _audioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            UpdateVolumeDataModel();
        }

        #endregion
    }
}