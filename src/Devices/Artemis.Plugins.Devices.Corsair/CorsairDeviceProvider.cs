using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using Artemis.Plugins.Devices.Corsair.Extensions;
using RGB.NET.Core;
using RGB.NET.Devices.Corsair;
using Serilog;
using System.Timers;
using RGBDeviceProvider = RGB.NET.Devices.Corsair.CorsairDeviceProvider;

namespace Artemis.Plugins.Devices.Corsair
{
    // ReSharper disable once UnusedMember.Global
    public class CorsairDeviceProvider : DeviceProvider
    {
        private readonly ILogger _logger;
        private readonly IDeviceService _deviceService;
        private readonly Plugin _plugin;
        private readonly Timer _restartTimer;

        public CorsairDeviceProvider(ILogger logger, IDeviceService deviceService, Plugin plugin)
        {
            _logger = logger;
            _deviceService = deviceService;
            _plugin = plugin;

            CanDetectLogicalLayout = true;
            CanDetectPhysicalLayout = true;
            CreateMissingLedsSupported = false;
            SuspendSupported = true;
            
            _restartTimer = new Timer(TimeSpan.FromHours(2));
            _restartTimer.Elapsed += RestartTimerOnElapsed;
            _restartTimer.Start();
        }

        public override RGBDeviceProvider RgbDeviceProvider => RGBDeviceProvider.Instance;

        private void RestartTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (!IsEnabled)
                return;

            Task.Run(async () =>
            {
                Disable();

                await Task.Delay(1000);

                Enable();
            });
        }

        public override void Enable()
        {
            if (_plugin.GetFeature<CorsairLegacyDeviceProvider>()!.IsEnabled)
                throw new ArtemisPluginException("The new Corsair device provider cannot be enabled while the legacy Corsair device provider is enabled");
            
            SdkHelper.EnsureSdkAvailable(_logger);

            RGBDeviceProvider.PossibleX64NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x64", "iCUESDK.x64_2019.dll"));
            RGBDeviceProvider.PossibleX86NativePaths.Add(Path.Combine(Plugin.Directory.FullName, "x86", "iCUESDK_2019.dll"));
            try
            {
                RgbDeviceProvider.SessionStateChanged += SessionStateChanged;
                RgbDeviceProvider.Exception += Provider_OnException;

                _deviceService.AddDeviceProvider(this);

                _logger.Debug("Corsair SDK details");
                _logger.Debug(" - Client version: {detail}", RgbDeviceProvider.SessionDetails.ClientVersion);
                _logger.Debug(" - Server version: {detail}", RgbDeviceProvider.SessionDetails.ServerVersion);
                _logger.Debug(" - Server-Host version: {detail}", RgbDeviceProvider.SessionDetails.ServerHostVersion);
            }
            catch (CUEException e)
            {
                throw new ArtemisPluginException($"Corsair SDK threw error: {e.Error}", e);
            }
        }



        public override void Disable()
        {
            _deviceService.RemoveDeviceProvider(this);

            RgbDeviceProvider.SessionStateChanged -= SessionStateChanged;
            RgbDeviceProvider.Exception -= Provider_OnException;
            RgbDeviceProvider.Dispose();
        }

        public override void Suspend()
        {
            // Kill iCUE because it freezes after sleep
            Process? icue = Process.GetProcessesByName("iCUE").FirstOrDefault();
            icue?.Kill();
        }

        public override string GetLogicalLayout(IKeyboard keyboard)
        {
            if (keyboard.DeviceInfo is CorsairKeyboardRGBDeviceInfo keyboardInfo)
            {
                // Simply use two-letter country code
                if (keyboardInfo.LogicalLayout == CorsairLogicalKeyboardLayout.US_Int)
                    return "US";
                return keyboardInfo.LogicalLayout.ToString();
            }

            return null;
        }

        public override string GetDeviceLayoutName(ArtemisDevice device)
        {
            // Pumps can have different amounts of LEDs but share the model name "PUMP", by including the LED count in the layout name
            // we can still have different layouts of each type of pump
            if (device.RgbDevice.DeviceInfo.Model.Equals("pump", StringComparison.InvariantCultureIgnoreCase))
                return $"PUMP-{device.RgbDevice.Count()}-ZONE.xml";

            // Early K95 Platinum models used the first key of columns of 3, use alternative layout
            if (device.IsEarlyK95Platinum())
                return $"K95 RGB PLATINUM-ALT-{device.PhysicalLayout.ToString().ToUpper()}.xml";

            return base.GetDeviceLayoutName(device);
        }
        
        private void Provider_OnException(object sender, ExceptionEventArgs args) => _logger.Debug(args.Exception, "Corsair Exception: {message}", args.Exception.Message);

        private void SessionStateChanged(object sender, CorsairSessionState state) => _logger.Debug("Corsair Session-State: {state}", state);
    }
}