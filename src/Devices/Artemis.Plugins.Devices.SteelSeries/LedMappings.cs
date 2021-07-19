using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RGB.NET.Core;
using RGB.NET.Devices.SteelSeries;

namespace Artemis.Plugins.Devices.SteelSeries
{
    public static class LedMappings
    {
        public static LedMapping<SteelSeriesLedId> MousepadTwoZone { get; } = new()
        {
            { LedId.Mousepad1, SteelSeriesLedId.ZoneOne },
            { LedId.Mousepad2, SteelSeriesLedId.ZoneTwo }
        };
    }
}
