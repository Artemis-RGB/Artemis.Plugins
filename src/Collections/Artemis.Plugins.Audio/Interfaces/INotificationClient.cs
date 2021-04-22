using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;
namespace Artemis.Plugins.LayerEffects.AudioVisualization.Interfaces
{
    public class INotificationClient : IMMNotificationClient
    {
        public delegate void DefaultDeviceChangedHandler();
        public delegate void DeviceStateChangedHandler();
        public delegate void DevicePropertyChangedHandler();
        public event DefaultDeviceChangedHandler DefaultDeviceChanged;
        public event DeviceStateChangedHandler DeviceStateChanged;
        public event DevicePropertyChangedHandler DevicePropertyChanged;

        public void OnDefaultDeviceChanged(DataFlow dataFlow, Role deviceRole, string defaultDeviceId)
        {
            if (DefaultDeviceChanged != null)
            {
                DefaultDeviceChanged();
            }
        }

        public void OnDeviceAdded(string deviceId)
        {
        }

        public void OnDeviceRemoved(string deviceId)
        {
        }

        public void OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
            if (DeviceStateChanged != null)
            {
                DeviceStateChanged();
            }
        }

        public INotificationClient()
        {
            if (System.Environment.OSVersion.Version.Major < 6)
            {
                throw new NotSupportedException("This functionality is only supported on Windows Vista or newer.");
            }
        }

        public void OnPropertyValueChanged(string deviceId, PropertyKey propertyKey)
        {
            if (DevicePropertyChanged != null)
            {
                DevicePropertyChanged();
            }
        }
    }
}