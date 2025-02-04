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
    public abstract class AudioEndpointVolumeModule : Module<AudioEndpointVolumeDataModel>
    {
        public override List<IModuleActivationRequirement> ActivationRequirements => null;

        #region Constructor

        public AudioEndpointVolumeModule(ILogger logger, NAudioDeviceEnumerationService naudioDeviceEnumerationService, NAudio.CoreAudioApi.Role role, NAudio.CoreAudioApi.DataFlow flow)
        {
            _logger = logger;
            _naudioDeviceEnumerationService = naudioDeviceEnumerationService;
            _role = role;
            _flow = flow;
        }

        #endregion

        #region Properties & Fields

        private readonly NAudioDeviceEnumerationService _naudioDeviceEnumerationService;
        internal readonly ILogger _logger;
        private readonly object _audioEventLock = new();
        private readonly List<DynamicChild<AudioChannelDataModel>> _channelsDataModels = new();
        internal readonly NAudio.CoreAudioApi.DataFlow _flow;
        internal readonly NAudio.CoreAudioApi.Role _role;
        internal bool _audioDeviceChanged;
        private float _lastMasterPeakVolumeNormalized;
        internal MMDevice _audioDevice;
        private AudioEndpointVolume _audioEndpointVolume;

        #endregion

        #region Plugin Methods
        
        public abstract override DataModelPropertyAttribute GetDataModelDescription();

        public override void Enable()
        {
            _naudioDeviceEnumerationService.NotificationClient.DefaultDeviceChanged += NotificationClient_DefaultDeviceChanged;
            UpdateAudioEndpointDevice(true);

            // We don't need mor than ~30 updates per second. It will keep CPU usage controlled. 60 or more updates per second could rise cpu usage
            AddTimedUpdate(TimeSpan.FromMilliseconds(33), UpdatePeakVolume, "UpdatePeakVolume");
        }

        public override void Disable()
        {
            _naudioDeviceEnumerationService.NotificationClient.DefaultDeviceChanged -= NotificationClient_DefaultDeviceChanged;
            _audioEndpointVolume?.Dispose();
            _audioEndpointVolume = null;
            FreeAudioEndpointDevice();
        }

        public override void Update(double deltaTime)
        {
            DataModel.TimeSinceLastSound += TimeSpan.FromSeconds(deltaTime);
            if (_audioDeviceChanged) UpdateAudioEndpointDevice();
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

            if (_audioDevice == null)
            {
                // To avoid null object exception on device change or don't update if there are no devices at all
                return;
            }

            // Update Main volume Peak
            lock (_audioEventLock) // To avoid query an Device/EndPoint that is not the current device anymore or has more or less channels
            {
                // Absolute master peak volume 
                float peakVolumeNormalized = _audioDevice?.AudioMeterInformation.MasterPeakValue ?? 0f;

                // Don't update datamodel if not neeeded
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
                AudioMeterInformationChannels channelsVolumeNormalized = _audioDevice?.AudioMeterInformation.PeakValues;

                //One more check because AudioEndpoint device can be null any time (device for example). If this is the case, just keep the actual values and update in the next update.
                if (channelsVolumeNormalized == null)
                    return;

                for (int i = 0; i < _channelsDataModels.Count && i < channelsVolumeNormalized.Count; i++)
                {
                    DynamicChild<AudioChannelDataModel> audioChannelDataModel = _channelsDataModels[i];
                    audioChannelDataModel.Value.PeakVolumeNormalized = channelsVolumeNormalized[i];
                    audioChannelDataModel.Value.PeakVolume = channelsVolumeNormalized[i] * 100f;
                }
            }
        }

        private void UpdateVolumeDataModel()
        {
            DataModel.VolumeChanged.Trigger();
            DataModel.VolumeNormalized = _audioEndpointVolume.MasterVolumeLevelScalar;
            DataModel.Volume = DataModel.VolumeNormalized * 100f;
            DataModel.ChannelCount = _audioEndpointVolume.Channels.Count;
            DataModel.DeviceState = _audioDevice.State;
            DataModel.Muted = _audioEndpointVolume.Mute;

            lock (_audioEventLock)
            {
                for (int i = 0; i < _channelsDataModels.Count; i++)
                {
                    DynamicChild<AudioChannelDataModel> audioChannelDataModel = _channelsDataModels[i];
                    float volumeNormalized = _audioEndpointVolume.Channels[i].VolumeLevelScalar;
                    audioChannelDataModel.Value.VolumeNormalized = volumeNormalized;
                    audioChannelDataModel.Value.Volume = volumeNormalized * 100f;
                }
            }
        }

        private void PopulateChannels()
        {
            DataModel.Channels.ClearDynamicChildren();
            _logger.Verbose($"AudioEndpoint device {_audioDevice.FriendlyName} channel list cleared");
            _logger.Verbose($"Preparing to populate {_audioEndpointVolume.Channels.Count} channels for device {_audioDevice.FriendlyName}");
            _channelsDataModels.Clear();
            for (int i = 0; i < _audioEndpointVolume.Channels.Count; i++)
            {
                _channelsDataModels.Add(
                    DataModel.Channels.AddDynamicChild(i.ToString(), new AudioChannelDataModel { ChannelIndex = i }, $"Channel {i}")
                );
                _logger.Verbose($"AudioEndpoint device {_audioDevice.FriendlyName} channel {i} populated");
            }
        }

        #endregion

        #region Audio Management methods

        private void NotificationClient_DefaultDeviceChanged()
        {
            _audioDeviceChanged = true;
            // Workarround. MMDevice won't dispose if Dispose() is called from 
            // non parent thread and NaudioNotificationClient callbacks come from another thread.
            // We will use Update() mrhod to dispose MMDevice from creator thread because this (NotificationClient_DefaultDeviceChanged()) is called from another thread
        }

        internal void UpdateAudioEndpointDevice(bool firstRun = false)
        {
            lock (_audioEventLock)
            {
                if (!firstRun) FreeAudioEndpointDevice();

                if (SetAudioEndpointDevice())
                {
                    PopulateChannels();
                    _audioDeviceChanged = false;
                    UpdateVolumeDataModel();
                }
            }
        }

        private void FreeAudioEndpointDevice()
        {
            string disposingAudioEndpointDeviceFriendlyName = _audioDevice?.FriendlyName ?? "Unknown";
            _audioDevice?.Dispose();
            _audioDevice = null;
            _logger.Verbose($"AudioEndpoint device {disposingAudioEndpointDeviceFriendlyName} unregistered as source device to fill AudioEndpoint volume data model");
            DataModel.Reset();
        }

        private bool SetAudioEndpointDevice()
        {
            _audioDevice = _naudioDeviceEnumerationService.GetDefaultAudioEndpoint(_flow, _role);

            if (_audioDevice == null)
            {
                _logger.Verbose("No audio device found with Console role. Audio peak volume won't be updated.");
                return false;
            }

            _audioEndpointVolume = _audioDevice.AudioEndpointVolume;

            _audioEndpointVolume.OnVolumeNotification += _audioEndpointVolume_OnVolumeNotification;
            DataModel.DefaultDeviceName = _audioDevice.FriendlyName;

            _logger.Verbose($"AudioEndpoint device {_audioDevice.FriendlyName} registered to to fill AudioEndpoint volume data model");
            return true;
        }

        private void _audioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            UpdateVolumeDataModel();
        }

        #endregion
    }
}