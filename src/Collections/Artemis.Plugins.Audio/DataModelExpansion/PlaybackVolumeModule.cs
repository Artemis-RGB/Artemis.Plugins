using System;
using System.Collections.Generic;
using Artemis.Core;
using Artemis.Core.Modules;
using Artemis.Plugins.Audio.DataModelExpansion.DataModels;
using Artemis.Plugins.Audio.Services;
using NAudio.CoreAudioApi;
using Serilog;

namespace Artemis.Plugins.Audio.DataModelExpansion
{
    [PluginFeature]
    public class PlaybackVolumeModule : Module<PlaybackVolumeDataModel>
    {
        public override List<IModuleActivationRequirement> ActivationRequirements => null;

        #region Constructor

        public PlaybackVolumeModule(ILogger logger, NAudioDeviceEnumerationService naudioDeviceEnumerationService)
        {
            _logger = logger;
            _naudioDeviceEnumerationService = naudioDeviceEnumerationService;
        }

        #endregion

        #region Properties & Fields

        private readonly NAudioDeviceEnumerationService _naudioDeviceEnumerationService;
        private readonly ILogger _logger;
        private readonly object _audioEventLock = new();
        private readonly List<DynamicChild<ChannelDataModel>> _channelsDataModels = new();
        private bool _playbackDeviceChanged;
        private float _lastMasterPeakVolumeNormalized;
        private MMDevice _playbackDevice;
        private AudioEndpointVolume _audioEndpointVolume;

        #endregion

        #region Plugin Methods
        
        public override DataModelPropertyAttribute GetDataModelDescription()
        {
            return new DataModelPropertyAttribute
            {
                Name = "Playback Volume",
                Description = "Contains information about audio playback"
            };
        }

        public override void Enable()
        {
            _naudioDeviceEnumerationService.NotificationClient.DefaultDeviceChanged += NotificationClient_DefaultDeviceChanged;
            UpdatePlaybackDevice(true);

            // We don't need mor than ~30 updates per second. It will keep CPU usage controlled. 60 or more updates per second could rise cpu usage
            AddTimedUpdate(TimeSpan.FromMilliseconds(33), UpdatePeakVolume, "UpdatePeakVolume");
        }

        public override void Disable()
        {
            _naudioDeviceEnumerationService.NotificationClient.DefaultDeviceChanged -= NotificationClient_DefaultDeviceChanged;
            _audioEndpointVolume?.Dispose();
            _audioEndpointVolume = null;
            FreePlaybackDevice();
        }

        public override void Update(double deltaTime)
        {
            DataModel.TimeSinceLastSound += TimeSpan.FromSeconds(deltaTime);
            if (_playbackDeviceChanged) UpdatePlaybackDevice();
        }

        #endregion

        #region Update DataModel Methods

        private void UpdatePeakVolume(double deltaTime)
        {
            if (IsEnabled == false)
            {
                // To avoid null object exception on _enumerator use after plugin is disabled.
                return;
            }

            // If no one one is using this DataModel, don't update this part.
            if (DataModel.ActivePaths.Count < 1)
            {
                return;
            }

            if (_playbackDevice == null)
            {
                // To avoid null object exception on device change or don't update if there are no devices at all
                return;
            }

            // Update Main volume Peak
            lock (_audioEventLock) // To avoid query an Device/EndPoint that is not the current device anymore or has more or less channels
            {

                // Absolute master peak volume 
                float peakVolumeNormalized = (float) _playbackDevice?.AudioMeterInformation.MasterPeakValue;

                // Don't update datamodel if not needed
                if (Math.Abs(_lastMasterPeakVolumeNormalized - peakVolumeNormalized) < 0.00001f)
                    return;

                // Sound detected. Reset timespan
                if (Math.Abs(_lastMasterPeakVolumeNormalized - 0.0) > 0.00001f) DataModel.TimeSinceLastSound = TimeSpan.Zero;

                DataModel.PeakVolumeNormalized = _lastMasterPeakVolumeNormalized = peakVolumeNormalized;
                DataModel.PeakVolume = peakVolumeNormalized * 100f;

                // Master peak volume relative to master volume
                DataModel.PeakVolumeRelativeNormalized = peakVolumeNormalized * DataModel.VolumeNormalized;
                DataModel.PeakVolumeRelative = peakVolumeNormalized * 100f * DataModel.VolumeNormalized;

                // Update Channels Peak
                AudioMeterInformationChannels channelsVolumeNormalized = _playbackDevice?.AudioMeterInformation.PeakValues;

                //One more check because Playback device can be null any time (device for example). If this is the case, just keep the actual values and update in the next update.
                if (channelsVolumeNormalized == null)
                    return;

                for (int i = 0; i < _channelsDataModels.Count && i < channelsVolumeNormalized.Count; i++)
                {
                    DynamicChild<ChannelDataModel> channelDataModel = _channelsDataModels[i];
                    channelDataModel.Value.PeakVolumeNormalized = channelsVolumeNormalized[i];
                    channelDataModel.Value.PeakVolume = channelsVolumeNormalized[i] * 100f;
                }

                // Populate Sessions DataModel. Note that if the node don't exists, conditions using these values will become invalid
                SessionCollection audioSessions = _playbackDevice?.AudioSessionManager.Sessions;
                for (int i = 0; i < audioSessions.Count; i++)
                {
                    AudioSessionControl session = audioSessions[i];

                    // Don't waste resources parsing system sessions data
                    if (session.IsSystemSoundsSession)
                        continue;

                    // Span Code thanks to Darth Affe https://github.com/DarthAffe
                    ReadOnlySpan<char> data = session.GetSessionInstanceIdentifier.AsSpan();
                    int pidStart = data.LastIndexOf("%") + 2;
                    int nameStart = data.LastIndexOf("\\") + 1;

                    ReadOnlySpan<char> nameSlice = data.Slice(nameStart);
                    int nameEnd = nameSlice.LastIndexOf(".");

                    // To avoid crashed with exe name that contains '.' character
                    string name = nameSlice.Slice(0, nameEnd).ToString().Replace('.', ' ');

                    if (!uint.TryParse(data.Slice(pidStart), out uint _))
                        continue;

                    // Ignore sessions without name. It may be a valid session but without a name it is useless for conditions system
                    if (string.IsNullOrEmpty(name))
                        continue;

                    // Don't remove unused sessions as these becomes just inactive for a while. It is faster to keep them than scan and remove them.
                    if (DataModel.Sessions.TryGetDynamicChild(name, out DynamicChild<SessionDataModel> sessionDataModel))
                    {
                        sessionDataModel.Value.Name = name;
                        sessionDataModel.Value.State = session.State;
                        sessionDataModel.Value.PeakVolume = session.AudioMeterInformation.MasterPeakValue * 100;
                        sessionDataModel.Value.PeakVolumeNormalized = session.AudioMeterInformation.MasterPeakValue;
                    }
                    else
                    {
                        DataModel.Sessions.AddDynamicChild(
                            name,
                            new SessionDataModel()
                            {
                                Name = name,
                                State = session.State,
                                PeakVolume = session.AudioMeterInformation.MasterPeakValue * 100,
                                PeakVolumeNormalized = session.AudioMeterInformation.MasterPeakValue
                            },
                            name
                            );
                    }
                }
            }
        }

        private void UpdateVolumeDataModel()
        {
            DataModel.VolumeChanged.Trigger();
            DataModel.VolumeNormalized = _audioEndpointVolume.MasterVolumeLevelScalar;
            DataModel.Volume = DataModel.VolumeNormalized * 100f;
            DataModel.ChannelCount = _audioEndpointVolume.Channels.Count;
            DataModel.DeviceState = _playbackDevice.State;
            DataModel.Muted = _audioEndpointVolume.Mute;

            lock (_audioEventLock)
            {
                for (int i = 0; i < _channelsDataModels.Count; i++)
                {
                    DynamicChild<ChannelDataModel> channelDataModel = _channelsDataModels[i];
                    float volumeNormalized = _audioEndpointVolume.Channels[i].VolumeLevelScalar;
                    channelDataModel.Value.VolumeNormalized = volumeNormalized;
                    channelDataModel.Value.Volume = volumeNormalized * 100f;
                }
            }
        }

        private void PopulateChannels()
        {
            DataModel.Channels.ClearDynamicChildren();
            _logger.Verbose($"Playback device {_playbackDevice.FriendlyName} channel list cleared");
            _logger.Verbose($"Preparing to populate {_audioEndpointVolume.Channels.Count} channels for device {_playbackDevice.FriendlyName}");
            _channelsDataModels.Clear();
            for (int i = 0; i < _audioEndpointVolume.Channels.Count; i++)
            {
                _channelsDataModels.Add(
                    DataModel.Channels.AddDynamicChild(i.ToString(), new ChannelDataModel { ChannelIndex = i }, $"Channel {i}")
                );
                _logger.Verbose($"Playback device {_playbackDevice.FriendlyName} channel {i} populated");
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
                if (!firstRun) FreePlaybackDevice();

                if (SetPlaybackDevice())
                {
                    PopulateChannels();
                    _playbackDeviceChanged = false;
                    UpdateVolumeDataModel();
                }
            }
        }

        private void FreePlaybackDevice()
        {
            string disposingPlaybackDeviceFriendlyName = _playbackDevice?.FriendlyName ?? "Unknown";
            _playbackDevice?.Dispose();
            _playbackDevice = null;
            _logger.Verbose($"Playback device {disposingPlaybackDeviceFriendlyName} unregistered as source device to fill Playback volume data model");
            DataModel.Reset();
        }

        private bool SetPlaybackDevice()
        {
            _playbackDevice = _naudioDeviceEnumerationService.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);

            if (_playbackDevice == null)
            {
                _logger.Verbose("No audio device found with Console role. Audio peak volume won't be updated.");
                return false;
            }

            _audioEndpointVolume = _playbackDevice.AudioEndpointVolume;

            _audioEndpointVolume.OnVolumeNotification += _audioEndpointVolume_OnVolumeNotification;
            DataModel.DefaultDeviceName = _playbackDevice.FriendlyName;

            _logger.Verbose($"Playback device {_playbackDevice.FriendlyName} registered to to fill Playback volume data model");
            return true;
        }

        private void _audioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            UpdateVolumeDataModel();
        }

        #endregion
    }
}