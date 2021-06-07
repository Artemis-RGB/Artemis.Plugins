using System;
using System.Runtime.InteropServices;
using Artemis.Core.Services;
using Artemis.Plugins.Audio.Interfaces;
using NAudio.CoreAudioApi;
using Serilog;

namespace Artemis.Plugins.Audio.Services
{
    /// <summary>
    /// This service will act as a container for a single MMDeviceEnumerator for all plugin features. 
    /// to bypass a limitation of NAudio that don’t allow create more than one instance of the enumerator
    /// from different threads
    /// </summary>
    public class NAudioDeviceEnumerationService : IPluginService, IDisposable
    {
        #region Properties & Fields

        private readonly ILogger _logger;
        private MMDeviceEnumerator _deviceEnumerator;
        private NotificationClient _notificationClient;

        #endregion

        #region Constructors

        public NAudioDeviceEnumerationService(ILogger logger)
        {
            _logger = logger;

            _deviceEnumerator = new MMDeviceEnumerator();
            _logger.Verbose("Audio device enumerator service created.");

            _notificationClient = new NotificationClient();
            _deviceEnumerator.RegisterEndpointNotificationCallback(_notificationClient);
            _logger.Verbose("Audio device event interface registered.");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Return the default INotificationClient used to register device changes event handlers. Return null if the Enumerator is not ready to be used
        /// </summary>
        public NotificationClient NotificationClient => _notificationClient;

        /// <summary>
        /// Get the current device for a given Flow direction and Device Role
        /// </summary>
        /// <param name="dataFlow">Audio data direction</param>
        /// <param name="role">Audio device role</param>
        /// <returns>Current device as MMDevice instance</returns>
        public MMDevice GetDefaultAudioEndpoint(DataFlow dataFlow, Role role)
        {
            try
            {
                return _deviceEnumerator.GetDefaultAudioEndpoint(dataFlow, role);
            }
            catch (AggregateException ea)
            {
                _logger.Error("AggregateException on GetCurrentDevice() " + ea);
            }
            catch (COMException ea)
            {
                if (ea.ErrorCode == -2147023728) // 0x80070490 No audio defive found.
                    _logger.Verbose("No audio device found to be returned as current playback device. Some plugins may stop working.");
            }
            catch (NullReferenceException noe)
            {
                _logger.Error("NullReferenceException on GetCurrentDevice() " + noe);
            }
            catch (Exception e)
            {
                _logger.Error("Another exception on GetCurrentDevice() " + e);
            }
            return null;
        }

        /// <summary>
        /// Get a collection of audio endpoints
        /// </summary>
        /// <param name="dataFlow">Audio data direction</param>
        /// <param name="swStateMask">Device state mask</param>
        /// <returns>Collection of audio endpoints</returns>
        public MMDeviceCollection EnumerateAudioEndPoints(DataFlow dataFlow, DeviceState swStateMask)
        {
            try
            {
                return _deviceEnumerator.EnumerateAudioEndPoints(dataFlow, swStateMask);
            }
            catch (AggregateException ea)
            {
                _logger.Error("AggregateException on EnumerateAudioEndPoints() " + ea);
            }
            catch (NullReferenceException noe)
            {
                _logger.Error("NullReferenceException on EnumerateAudioEndPoints() " + noe);
            }
            catch (Exception e)
            {
                _logger.Error("Another exception on EnumerateAudioEndPoints() " + e);
            }
            return null;
        }

        public void Dispose()
        {
            _deviceEnumerator.UnregisterEndpointNotificationCallback(_notificationClient);
            _logger.Verbose("Audio device event interface unregistered.");
            _deviceEnumerator?.Dispose();
            _deviceEnumerator = null;
            _logger.Verbose("Audio device enumerator service disposed.");
        }

        #endregion

    }
}