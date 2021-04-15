using RGB.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static TasmotaRGBDeviceProvider Instance => _instance ?? new TasmotaRGBDeviceProvider();

        #endregion

        #region Methods

        protected override void InitializeSDK()
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<IRGBDevice> LoadDevices()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
