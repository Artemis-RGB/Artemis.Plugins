using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.Plugins.Devices.Wooting.Services.AnalogService;
using Grpc.Core;
using Grpc.Net.Client;
using Serilog;
using WootingAnalogSdk;

namespace Artemis.Plugins.Devices.Wooting;

public class WootingAnalogInputProvider : InputProvider
{
    private readonly ILogger _logger;
    private readonly IInputService _inputService;
    private readonly IDeviceService _deviceService;
    private readonly Lock _lock;
    private readonly GrpcChannel _channel;
    private readonly AnalogSdkService.AnalogSdkServiceClient _client;
    private readonly CancellationTokenSource _cts;
    private readonly List<Task> _tasks;
    private readonly ConcurrentDictionary<string, int> _identified;

    public WootingAnalogInputProvider(ILogger logger, IInputService inputService, IDeviceService deviceService)
    {
        _logger = logger;
        _inputService = inputService;
        _deviceService = deviceService;
        _lock = new Lock();
        _channel = GrpcChannel.ForAddress("http://127.0.0.1:50051");
        _client = new AnalogSdkService.AnalogSdkServiceClient(_channel);
        _cts = new CancellationTokenSource();
        _tasks = new List<Task>();
        _identified = new ConcurrentDictionary<string, int>();

        foreach (AnalogGetDevicesResponse.Types.AnalogDevice deviceInfo in _client.GetConnectedDevices(new AnalogGetDevicesRequest()).Devices)
        {
            _tasks.Add(Task.Run(() => RunDevice(deviceInfo)));
        }
    }

    private async Task RunDevice(AnalogGetDevicesResponse.Types.AnalogDevice deviceInfo)
    {
        AsyncServerStreamingCall<AnalogStreamResponse> stream = _client.AnalogStream(new AnalogStreamRequest { Deviceid = deviceInfo.Id });

        Dictionary<KeyboardKey, bool> cache = new();

        ArtemisDevice? device = null;

        await foreach (AnalogStreamResponse response in stream.ResponseStream.ReadAllAsync(_cts.Token))
        {
            if (response.Data.Count == 0)
                continue;

            device ??= _inputService.GetDeviceByIdentifier(this, deviceInfo.SerialNumber, InputDeviceType.Keyboard);

            if (device == null)
                continue;

            foreach (AnalogEntry entry in response.Data)
            {
                _identified.GetOrAdd(deviceInfo.SerialNumber, _ =>
                {
                    //we haven't identified this device yet. do the hack to identify it.

                    //if the identifier contains the serial number, it's likely the right device
                    ArtemisDevice mostLikelyDevice = _deviceService.Devices.FirstOrDefault(x => x.Identifier.Contains(deviceInfo.SerialNumber, StringComparison.InvariantCultureIgnoreCase));
                    if (mostLikelyDevice == null)
                        return 1;

                    //since we now know the device, we can identify it without user interaction

                    //we set this device as active in the input service, so that the next identifier it receives should be the correct one.
                    _inputService.IdentifyDevice(mostLikelyDevice);

                    _logger.Information("Identified Wooting device {DeviceName} with serial number {SerialNumber}", mostLikelyDevice.Identifier, deviceInfo.SerialNumber);
                    OnIdentifierReceived(deviceInfo.SerialNumber, InputDeviceType.Keyboard);
                    _deviceService.SaveDevice(mostLikelyDevice);

                    device = mostLikelyDevice;

                    return 1;
                });


                if (!WootingAnalogLedMapping.InputKeyCodesReversed.TryGetValue((short)entry.Key, out KeyboardKey keyboardKey))
                    continue;

                bool actuated = entry.Value > 0.5f;

                if (!cache.TryGetValue(keyboardKey, out bool previouslyActuated))
                {
                    previouslyActuated = false;
                    cache[keyboardKey] = actuated;
                }

                if (previouslyActuated == actuated)
                    continue;

                cache[keyboardKey] = actuated;
                OnKeyboardDataReceived(device, keyboardKey, actuated);
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
    }
}