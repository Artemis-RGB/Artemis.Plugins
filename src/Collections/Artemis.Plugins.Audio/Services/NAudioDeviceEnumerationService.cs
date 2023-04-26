using System;
using System.Runtime.InteropServices;
using Artemis.Plugins.Audio.Interfaces;
using Artemis.Plugins.Audio.LayerEffects.AudioCapture;
using NAudio.CoreAudioApi;
using Serilog;

namespace Artemis.Plugins.Audio.Services;

/// <summary>
/// This service will act as a container for a single MMDeviceEnumerator for all plugin features. 
/// to bypass a limitation of NAudio that don’t allow create more than one instance of the enumerator
/// from different threads
/// </summary>
public class NAudioDeviceEnumerationService : IAudioEnumerationService
{
    private readonly ILogger _logger;
    private readonly NotificationClient _notificationClient;
    private readonly MMDeviceEnumerator _deviceEnumerator;

    public NAudioDeviceEnumerationService(ILogger logger)
    {
        _logger = logger;

        _deviceEnumerator = new MMDeviceEnumerator();
        _logger.Verbose("Audio device enumerator service created.");

        _notificationClient = new NotificationClient();
        _deviceEnumerator.RegisterEndpointNotificationCallback(_notificationClient);
        _logger.Verbose("Audio device event interface registered.");
        _notificationClient.DefaultDeviceChanged += NotificationClientOnDefaultDeviceChanged;
    }

    private void NotificationClientOnDefaultDeviceChanged(object sender, EventArgs e)
    {
        DefaultDeviceChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Get the current device for a given Flow direction and Device Role
    /// </summary>
    /// <param name="dataFlow">Audio data direction</param>
    /// <param name="role">Audio device role</param>
    /// <returns>Current device as MMDevice instance</returns>
    public MMDevice GetDefaultAudioEndpoint()
    {
        try
        {
            return _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
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

    public IAudioInput CreateDefaultAudioInput(bool compatibilityMode)
    {
        return new NAudioAudioInput(this, compatibilityMode, _logger);
    }

    public event EventHandler DefaultDeviceChanged;

    public void Dispose()
    {
        _notificationClient.DefaultDeviceChanged -= NotificationClientOnDefaultDeviceChanged;
        _deviceEnumerator.UnregisterEndpointNotificationCallback(_notificationClient);
        _logger.Verbose("Audio device event interface unregistered.");
        _deviceEnumerator?.Dispose();
        _logger.Verbose("Audio device enumerator service disposed.");
    }
}