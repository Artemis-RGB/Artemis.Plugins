using RGB.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Artemis.Plugins.Tasmota.RGB.NET
{
    public class TasmotaDevice : AbstractRGBDevice<TasmotaDeviceInfo>
    {
        public TasmotaDevice(TasmotaDeviceInfo deviceInfo, TasmotaUpdateQueue updateQueue) : base(deviceInfo, updateQueue)
        {
            InitializeLayout();
        }

        private void InitializeLayout()
        {
            AddLed(LedId.Custom1, new Point(0, 0), new Size(10, 10));
        }
        protected override object GetLedCustomData(LedId ledId) => (ledId, 0x00);

        protected override void UpdateLeds(IEnumerable<Led> ledsToUpdate) => UpdateQueue.SetData(GetUpdateData(ledsToUpdate.Take(1)));
    }
}