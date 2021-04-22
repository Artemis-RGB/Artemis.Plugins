using System;
using Artemis.Core.Services;
using Artemis.Plugins.LayerEffects.AudioVisualization.Interfaces;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace Artemis.Plugins.LayerEffects.AudioVisualization.Services
{
    /// <summary>
    /// This service will act as a container for a single MMDeviceEnumerator for all plugin features. 
    /// to bypass a limitation of NAudio that don’t allow create more than one instance of the enumerator
    /// from different threads
    /// </summary>
    public class NAudioDeviceEnumerationService : IPluginService, IDisposable
    {
        #region Properties & Fields

        private MMDeviceEnumerator _deviceEnumerator;
        private INotificationClient _notificationClient;
        private IMMNotificationClient _notifyClient;

        /// <summary>
        /// Return the only one MMDeviceEnumerator instance for all plugin features.
        /// </summary>
        public MMDeviceEnumerator Enumerator => _deviceEnumerator;

        /// <summary>
        /// Return the default INotificationClient used to register device changes event handlers
        /// </summary>
        public INotificationClient NotificationClient => _notificationClient;

        /// <summary>
        /// Get the current device for a given Flow direction and Device Role
        /// </summary>
        public MMDevice GetCurrentDevice(DataFlow dataFlow, Role role)
        {
            return _deviceEnumerator.GetDefaultAudioEndpoint(dataFlow, role);
        }

        #endregion

        #region Constructors

        public NAudioDeviceEnumerationService()
        {
            _deviceEnumerator = new MMDeviceEnumerator();
            _notificationClient = new INotificationClient();
            _notifyClient = (IMMNotificationClient)_notificationClient;
            _deviceEnumerator.RegisterEndpointNotificationCallback(_notifyClient);
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            // TODO: Exception if plugin is disabled.

            _deviceEnumerator.Dispose();
            _deviceEnumerator = null;
        }
        #endregion
    }
}