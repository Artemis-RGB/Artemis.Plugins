using RGB.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Artemis.Plugins.Tasmota.RGB.NET
{
    public class TasmotaRGBDeviceProvider : AbstractRGBDeviceProvider
    {
        #region Constructors

        public TasmotaRGBDeviceProvider()
        {
            if (_instance != null) throw new InvalidOperationException($"There can be only one instance of type {nameof(TasmotaRGBDeviceProvider)}");
            _instance = this;
        }

        #endregion

        #region Properties & Fields

        private static TasmotaRGBDeviceProvider _instance;

        private List<String> stripsIpAddress;

        public static TasmotaRGBDeviceProvider Instance => _instance ?? new TasmotaRGBDeviceProvider();

        private TasmotaUpdateQueue? _tasmotaUpdateQueue;


        #endregion

        #region Methods

        protected override void InitializeSDK()
        {
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        protected override IEnumerable<IRGBDevice> LoadDevices()
        {
            stripsIpAddress = new List<string>
            {
                "192.168.178.31"
            };
            foreach (string ipAddress in stripsIpAddress)
            {
                TasmotaLight light = new();
                bool add = true;
                try
                {
                    light.ConnectAsync(IPAddress.Parse(ipAddress));
                    light.TurnOnAsync();
                    light.AutoRefreshEnabled = true;

                }
                catch
                {
                    add = false;
                }
                if (add)
                    yield return new TasmotaDevice(new TasmotaDeviceInfo(RGBDeviceType.Unknown, "RGB Strip", "192.168.1.42"), new TasmotaUpdateQueue(GetUpdateTrigger(), light));
            }
        }

        #endregion
    }
}
