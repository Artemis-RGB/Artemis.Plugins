using System;
using System.Collections.Generic;
using RGB.NET.Core;

namespace Artemis.Plugins.Devices.SoundBlasterX;

// A device this plugin knows how to lay out. Only LED strips for now: LEDs in one row, LED 0 at the left end,
// numbered from FirstLed. Use the LED IDs of the device type (Speaker1.. for a speaker), not LedStripe1..:
// the Chroma brush maps LedStripe1-5 to Chroma Link zones by ID. Keyboards and mice (Vanguard K08,
// Siege M04) need a key/zone mapping instead; Aurora's Devices/Creative/SoundBlasterMappings.cs has one.
internal sealed record SoundBlasterXModel(string Model, RGBDeviceType Type, LedId FirstLed, float LedWidth, float LedHeight);

// RGB.NET side: one device per supported Creative device found, each with its own LED manager instance.
public sealed class SoundBlasterXRGBDeviceProvider : AbstractRGBDeviceProvider
{
    private const ushort CreativeVendorId = 0x041E;

    // Product IDs from CTSDK/Include/CTLEDCommon.h. Add a device here once its LED layout is known and tested.
    private static readonly Dictionary<ushort, SoundBlasterXModel> SupportedDevices = new()
    {
        // Tested: 49 LEDs, LED 0 at the left end. About 600 mm wide.
        [0x3247] = new("Sound BlasterX Katana", RGBDeviceType.Speaker, LedId.Speaker1, 12, 10),
    };

    // Artemis renders at about 25-60 fps; these devices don't need more than this.
    private const double UpdateRateHardLimit = 1.0 / 30;

    private readonly string _nativeDirectory;
    private readonly List<CreativeLedManager> _managers = [];
    private CreativeLibrary? _library;

    public SoundBlasterXRGBDeviceProvider(string nativeDirectory) : base(UpdateRateHardLimit)
    {
        _nativeDirectory = nativeDirectory;
    }

    protected override void InitializeSDK()
    {
        _library = new CreativeLibrary(_nativeDirectory);
        // Kept open until Reset, like Aurora's scanner; devices are opened in their own instances.
        _managers.Add(_library.CreateManager());
    }

    protected override IEnumerable<IRGBDevice> LoadDevices()
    {
        List<IRGBDevice> devices = [];
        foreach (var found in _managers[0].EnumerateDevices())
        {
            if (found.VendorId != CreativeVendorId || found.Leds == 0 || !SupportedDevices.TryGetValue(found.ProductId, out var model))
                continue;

            try
            {
                var manager = _library!.CreateManager();
                _managers.Add(manager);
                manager.Open(found);
                var queue = new SoundBlasterXUpdateQueue(GetUpdateTrigger(), manager, found.Leds, ex => Throw(ex));
                devices.Add(new SoundBlasterXRGBDevice(new SoundBlasterXRGBDeviceInfo(model), queue, model, found.Leds));
            }
            catch (Exception ex)
            {
                // One device failing to open shouldn't keep the others from loading.
                Throw(ex);
            }
        }

        return devices;
    }

    // Base Reset stops the update trigger without waiting for it; each manager's lock keeps a last
    // colour update from running into the shutdown, and later ones are ignored.
    protected override void Reset()
    {
        base.Reset();
        foreach (var manager in _managers)
            manager.Dispose();
        _managers.Clear();
        _library?.Dispose();
        _library = null;
    }
}

public sealed class SoundBlasterXRGBDeviceInfo : IRGBDeviceInfo
{
    internal SoundBlasterXRGBDeviceInfo(SoundBlasterXModel model)
    {
        DeviceType = model.Type;
        Model = model.Model;
        DeviceName = DeviceHelper.CreateDeviceName(Manufacturer, Model);
    }

    public RGBDeviceType DeviceType { get; }
    public string DeviceName { get; }
    public string Manufacturer => "Creative";
    public string Model { get; }
    public object? LayoutMetadata { get; set; }
}

public sealed class SoundBlasterXRGBDevice : AbstractRGBDevice<SoundBlasterXRGBDeviceInfo>
{
    internal SoundBlasterXRGBDevice(SoundBlasterXRGBDeviceInfo info, IUpdateQueue queue, SoundBlasterXModel model, int leds) : base(info, queue)
    {
        for (var i = 0; i < leds; i++)
            AddLed(model.FirstLed + i, new Point(i * model.LedWidth, 0), new Size(model.LedWidth, model.LedHeight), i);
    }
}

// RGB.NET hands over only the LEDs that changed; the Creative call sets all of them, so the last
// colour of every LED is kept here.
internal sealed class SoundBlasterXUpdateQueue : UpdateQueue
{
    private readonly CreativeLedManager _manager;
    private readonly CTColour[] _colours;
    private readonly Action<Exception> _onError;
    private bool _failing;

    public SoundBlasterXUpdateQueue(IDeviceUpdateTrigger trigger, CreativeLedManager manager, int leds, Action<Exception> onError) : base(trigger)
    {
        _manager = manager;
        _colours = new CTColour[leds];
        _onError = onError;
    }

    protected override bool Update(ReadOnlySpan<(object key, Color color)> dataSet)
    {
        foreach (var (key, color) in dataSet)
            if (key is int index && (uint)index < (uint)_colours.Length)
                _colours[index] = new CTColour { R = color.GetR(), G = color.GetG(), B = color.GetB(), A = 255 };

        try
        {
            _manager.SetColours(_colours);
            _failing = false;
            return true;
        }
        catch (Exception ex)
        {
            // Report once per run of failures (e.g. the device unplugged), not on every frame.
            if (!_failing)
                _onError(ex);
            _failing = true;
            return false;
        }
    }
}
