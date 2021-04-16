using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using Artemis.Plugins.Tasmota.RGB.NET;
using Serilog;
using System.IO;

namespace Artemis.Plugins.Tasmota
{
    // This is your Artemis device provider, all it really does is act as a bridge between RGB.NET and Artemis
    // You will not write any device logic in here, refer to the RgbNetDeviceProvider project instead    
    public class TasmotaDeviceProvider : DeviceProvider
    {
        private readonly ILogger _logger;
        private readonly IRgbService _rgbService;

        public TasmotaDeviceProvider(IRgbService rgbService, ILogger logger) : base(TasmotaRGBDeviceProvider.Instance)
        {
            _rgbService = rgbService;
            _logger = logger;
        }

        public override void Enable()
        {
            _rgbService.AddDeviceProvider(RgbDeviceProvider);
        }


        public override void Disable()
        {
            TasmotaRGBDeviceProvider tasmotaProvider = (TasmotaRGBDeviceProvider)RgbDeviceProvider;
            tasmotaProvider.Dispose();
            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
        }
    }
}