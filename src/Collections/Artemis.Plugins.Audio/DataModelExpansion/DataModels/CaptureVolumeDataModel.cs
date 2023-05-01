using System;
using Artemis.Core;
using Artemis.Core.Modules;
using NAudio.CoreAudioApi;

namespace Artemis.Plugins.Audio.DataModelExpansion.DataModels
{
    public class CaptureVolumeDataModel : DataModel
    {
        [DataModelProperty(Description ="Name of the current capture device.")]
        public string DefaultDeviceName { get; set; }
        [DataModelProperty(Description = "Channel count of the the current capture device.")]
        public int ChannelCount { get; set; }
        [DataModelProperty(Description = "Master volume of the current capture device.")]
        public float Volume { get; set; }
        [DataModelProperty(Description = "Normalized (value between 0..1) master volume of the current capture device.")]
        public float VolumeNormalized { get; set; }
        [DataModelProperty(Description = "Mute state of the current capture device.")]
        public bool Muted { get; set; }
        [DataModelProperty(Description = "Real time master peak volume of the current capture device.")]
        public float PeakVolume { get; set; }
        [DataModelProperty(Description = "Real time normalized (value between 0..1) master peak volume of the current capture device.")]
        public float PeakVolumeNormalized { get; set; }
        [DataModelProperty(Description = "Master channel relative real time master peak volume of the current capture device.")]
        public float PeakVolumeRelative { get; set; }
        [DataModelProperty(Description = "Master channel relative real time normalized (value between 0..1) master peak volume of the current capture device.")]
        public float PeakVolumeRelativeNormalized { get; set; }
        [DataModelProperty(Description = "State of the current capture device.")]
        public DeviceState DeviceState { get; set; }
        [DataModelProperty(Description = "Time since las played sound on the current capture device.")]
        public TimeSpan TimeSinceLastSound { get; set; }
        [DataModelProperty(Description = "Event triggered when current capture device master volume is changed.")]
        public DataModelEvent VolumeChanged { get; set; } = new DataModelEvent();
        public ChannelsDataModel Channels { get; set; } = new ChannelsDataModel();

        public void Reset()
        {
            DefaultDeviceName = "No device detected";
            ChannelCount = 0;
            Volume = 0;
            VolumeNormalized = 0;
            Muted = false;
            PeakVolume = 0;
            PeakVolumeNormalized = 0;
            PeakVolumeRelative = 0;
            PeakVolumeRelativeNormalized = 0;
            DeviceState = DeviceState.NotPresent;
        }
    }

    /* the following can be found in PlaybackVolumeDataModel.cs
    public class ChannelsDataModel : DataModel { }
    public class ChannelDataModel : DataModel
    {
        public int ChannelIndex { get; set; }
        public float Volume { get; set; }
        public float VolumeNormalized { get; set; }
        public float PeakVolume { get; set; }
        public float PeakVolumeNormalized { get; set; }
    }
    */
}