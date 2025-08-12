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
    private readonly IDeviceService _deviceService;
    private readonly GrpcChannel _channel;
    private readonly AnalogSdkService.AnalogSdkServiceClient _client;
    private readonly CancellationTokenSource _cts;
    private readonly List<Task> _tasks;

    public WootingAnalogInputProvider(IDeviceService deviceService)
    {
        _deviceService = deviceService;
        _channel = GrpcChannel.ForAddress("http://127.0.0.1:50051");
        _client = new AnalogSdkService.AnalogSdkServiceClient(_channel);
        _cts = new CancellationTokenSource();
        _tasks = new List<Task>();

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

            device ??= _deviceService.Devices.FirstOrDefault(x => x.Identifier.Contains(deviceInfo.SerialNumber, StringComparison.InvariantCultureIgnoreCase));

            if (device == null)
                continue;

            foreach (AnalogEntry entry in response.Data)
            {
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
        if (disposing)
        {
            _cts.Cancel();
            _channel.ShutdownAsync().GetAwaiter().GetResult();
            
            foreach (Task task in _tasks)
            {
                try
                {
                    task.GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error while disposing WootingAnalogInputProvider task");
                }
            }
        }

        base.Dispose(disposing);
    }
}