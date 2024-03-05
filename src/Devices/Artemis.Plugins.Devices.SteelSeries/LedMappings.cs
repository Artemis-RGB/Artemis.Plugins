using RGB.NET.Core;
using RGB.NET.Devices.SteelSeries;

namespace Artemis.Plugins.Devices.SteelSeries
{
    public static class LedMappings
    {
        /// <summary>
        /// Gets the mapping for eight-zone keyboards
        /// </summary>
        public static LedMapping<SteelSeriesLedId> KeyboardEightZone { get; } = new()
        {
            { LedId.Keyboard_Custom1, SteelSeriesLedId.ZoneOne },
            { LedId.Keyboard_Custom2, SteelSeriesLedId.ZoneTwo },
            { LedId.Keyboard_Custom3, SteelSeriesLedId.ZoneThree },
            { LedId.Keyboard_Custom4, SteelSeriesLedId.ZoneFour },
            { LedId.Keyboard_Custom5, SteelSeriesLedId.ZoneFive },
            { LedId.Keyboard_Custom6, SteelSeriesLedId.ZoneSix },
            { LedId.Keyboard_Custom7, SteelSeriesLedId.ZoneSeven },
            { LedId.Keyboard_Custom8, SteelSeriesLedId.ZoneEight }
        };
    }
}
