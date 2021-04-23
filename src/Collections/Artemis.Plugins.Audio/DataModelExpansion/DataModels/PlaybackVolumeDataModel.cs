﻿using Artemis.Core;
using Artemis.Core.DataModelExpansions;
using NAudio.CoreAudioApi;

namespace Artemis.Plugins.Audio.DataModelExpansions.PlaybackVolume
{
    public class PlaybackVolumeDataModel : DataModel
    {
        [DataModelProperty(Description ="Name of the current playback device.")]
        public string DefaultDeviceName { get; set; }
        [DataModelProperty(Description = "Channel count of the the current playback device.")]
        public int ChannelCount { get; set; }
        [DataModelProperty(Description = "Master volume of the current playback device.")]
        public float Volume { get; set; }
        [DataModelProperty(Description = "Normalized (value between 0..1) master volume of the current playback device.")]
        public float VolumeNormalized { get; set; }
        [DataModelProperty(Description = "Mute state of the current playback device.")]
        public bool Muted { get; set; }
        [DataModelProperty(Description = "Real time master peak volume of the current playback device.")]
        public float PeakVolume { get; set; }
        [DataModelProperty(Description = "Real time normalized (value between 0..1) master peak volume of the current playback device.")]
        public float PeakVolumeNormalized { get; set; }
        [DataModelProperty(Description = "Master channel relative real time master peak volume of the current playback device.")]
        public float PeakVolumeRelative { get; set; }
        [DataModelProperty(Description = "Master channel relative real time normalized (value between 0..1) master peak volume of the current playback device.")]
        public float PeakVolumeRelativeNormalized { get; set; }
        [DataModelProperty(Description = "State of the current playback device.")]
        public DeviceState DeviceState { get; set; }
        public ChannelsDataModel Channels { get; set; } = new ChannelsDataModel();
        [DataModelProperty(Description = "Event triggered when current playback device master volume is changed.")]
        public DataModelEvent VolumeChanged { get; set; } = new DataModelEvent();
        [DataModelProperty(Description = "Event triggered when current playback plays a sound.")]
        public DataModelEvent SoundPlayed { get; set; } = new DataModelEvent();
    }

    public class ChannelsDataModel : DataModel { }
    public class ChannelDataModel : DataModel
    {
        public int ChannelIndex { get; set; }
        public float Volume { get; set; }
        public float VolumeNormalized { get; set; }
        public float PeakVolume { get; set; }
        public float PeakVolumeNormalized { get; set; }
    }
}