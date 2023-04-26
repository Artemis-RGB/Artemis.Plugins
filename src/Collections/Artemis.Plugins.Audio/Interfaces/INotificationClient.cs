using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;
namespace Artemis.Plugins.Audio.Interfaces
{
    public class NotificationClient : IMMNotificationClient
    {
        public event EventHandler DefaultDeviceChanged;
        public event EventHandler DeviceStateChanged;
        public event EventHandler DevicePropertyChanged;

        public void OnDefaultDeviceChanged(DataFlow dataFlow, Role deviceRole, string defaultDeviceId)
        {
            DefaultDeviceChanged?.Invoke(this, EventArgs.Empty);
        }

        public void OnDeviceAdded(string deviceId)
        {
        }

        public void OnDeviceRemoved(string deviceId)
        {
        }

        public void OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
            DeviceStateChanged?.Invoke(this, EventArgs.Empty);
        }

        public NotificationClient()
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                throw new NotSupportedException("This functionality is only supported on Windows Vista or newer.");
            }
        }

        public void OnPropertyValueChanged(string deviceId, PropertyKey propertyKey)
        {
            DevicePropertyChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}