using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Artemis.Plugins.Devices.SoundBlasterX;

// Creative's x64 LED manager DLLs (CTSDK/Redist/x64 from https://github.com/rajkosto/SBAuroraR.NET, the same
// files Aurora ships), loaded once by full path so the manager finds its sibling DLLs. Each ICTLEDMgr instance
// from CreateManager handles one device, as Aurora's SoundBlasterX device does.
internal sealed unsafe class CreativeLibrary : IDisposable
{
    private static readonly Guid ClsidLedMgr = new("3370DE2D-576E-421D-9D04-E01B84D0E3EA");
    private static readonly Guid IidLedMgr = new("1185BF2F-BEF2-4477-8B6F-A40D529D22EB");

    private static readonly string[] NativeLibraries =
        ["CTIntrfu.dll", "CTHID.dll", "CTHIDRpA.dll", "ClplLEDController.dll", "CTDrvLedMgr.dll", "CTLEDMgr.dll"];

    private readonly string _directory;
    private readonly List<IntPtr> _libraries = [];
    private readonly delegate* unmanaged<Guid*, void*, uint, Guid*, char*, char*, char*, char*, void**, uint> _create;

    public CreativeLibrary(string directory)
    {
        _directory = directory;
        try
        {
            foreach (var name in NativeLibraries)
                _libraries.Add(NativeLibrary.Load(Path.Combine(directory, name)));
            _create = (delegate* unmanaged<Guid*, void*, uint, Guid*, char*, char*, char*, char*, void**, uint>)
                NativeLibrary.GetExport(_libraries[0], "CTCreateInstanceEx");
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    public CreativeLedManager CreateManager()
    {
        var clsid = ClsidLedMgr;
        var iid = IidLedMgr;
        void* mgr;
        uint created;
        fixed (char* ledMgrPath = Path.Combine(_directory, "CTLEDMgr.dll"))
            created = _create(&clsid, null, 0, &iid, null, null, ledMgrPath, null, &mgr);
        if (created != 0)
            throw new InvalidOperationException($"CTCreateInstanceEx failed: {created}");
        return new CreativeLedManager(mgr);
    }

    // Call after every manager is disposed.
    public void Dispose()
    {
        if (_libraries.Count == 0)
            return;
        if ((void*)_create != null)
            ((delegate* unmanaged<void>)NativeLibrary.GetExport(_libraries[0], "CTFreeUnusedLibrariesEx"))();
        for (var i = _libraries.Count - 1; i >= 0; i--)
            NativeLibrary.Free(_libraries[i]);
        _libraries.Clear();
    }
}

// One ICTLEDMgr instance, called through its vtable with C# function pointers; slots and structs follow the
// CTSDK headers (CTSDK/Include in https://github.com/rajkosto/SBAuroraR.NET). The payload (one LED per group,
// static pattern, one colour layer) follows Aurora's SoundBlasterX device (https://github.com/Aurora-RGB/Aurora).
// All calls are serialised: colours come from RGB.NET's update thread, Dispose from Artemis.
internal sealed unsafe class CreativeLedManager : IDisposable
{
    // ICTLEDMgr vtable slots, after IUnknown's QueryInterface (0), AddRef (1) and Release (2).
    private const int SlotRelease = 2, SlotInitialize = 3, SlotOpen = 5, SlotPrepareLedInfo = 7,
        SlotExecuteCommand = 8, SlotClose = 9, SlotShutdown = 11, SlotEnumConnectedDevices = 13;

    private const uint InterfaceVersion = 2;
    private const uint InfoInitialize = 1, InfoFillupAll = 2, InfoCleanup = 9;
    private const uint CommandSetLedSettings = 2;
    private const uint PatternStatic = 3;
    private const uint NotApplicable = 0xFFFFFFFF;

    private readonly Lock _lock = new();
    private void* _mgr;
    private bool _initialized;
    private ushort _openLeds;

    internal CreativeLedManager(void* mgr)
    {
        _mgr = mgr;
        try
        {
            Check(((delegate* unmanaged<void*, uint, uint, int>)Slot(SlotInitialize))(_mgr, InterfaceVersion, 0), "Initialize");
            _initialized = true;
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    public List<CreativeDevice> EnumerateDevices()
    {
        var enumerate = (delegate* unmanaged<void*, uint, ushort*, ushort*, char*, uint*, char*, uint*, ushort*, ushort*, char*, uint*, uint, int>)
            Slot(SlotEnumConnectedDevices);
        const int bufferLength = 256;
        char* serial = stackalloc char[bufferLength];
        char* instance = stackalloc char[bufferLength];
        char* name = stackalloc char[bufferLength];
        List<CreativeDevice> devices = [];

        lock (_lock)
            for (uint index = 0; index < 64; index++)
            {
                new Span<char>(serial, bufferLength).Clear();
                new Span<char>(instance, bufferLength).Clear();
                new Span<char>(name, bufferLength).Clear();
                ushort vid, pid, ledInfoFlag, leds;
                uint serialLength = bufferLength - 1, instanceLength = bufferLength - 1, nameLength = bufferLength - 1;
                if (enumerate(_mgr, index, &vid, &pid, serial, &serialLength, instance, &instanceLength, &ledInfoFlag, &leds, name, &nameLength, 0) < 0)
                    break;
                devices.Add(new CreativeDevice(vid, pid, new string(serial), new string(instance), new string(name), ledInfoFlag, leds));
            }

        return devices;
    }

    // One device per manager instance.
    public void Open(CreativeDevice device)
    {
        lock (_lock)
        {
            uint detail;
            int hr;
            fixed (char* serial = device.Serial)
            fixed (char* instance = device.Instance)
                hr = ((delegate* unmanaged<void*, ushort, ushort, char*, char*, ushort, ushort, int, uint*, uint, int>)Slot(SlotOpen))(
                    _mgr, device.VendorId, device.ProductId, serial, instance, device.LedInfoFlag, device.Leds, 0, &detail, 0);
            Check(hr, $"Open (detail {detail})");
            _openLeds = device.Leds;
        }
    }

    // One LED per group, all static, one colour layer, not saved to the device (Persistent = FALSE).
    // This puts the device in per-LED (local) pattern mode; Dispose puts it back in global mode.
    public void SetColours(ReadOnlySpan<CTColour> colours)
    {
        lock (_lock)
        {
            if (_openLeds == 0)
                return;

            var leds = Math.Min(_openLeds, colours.Length);
            var prepare = (delegate* unmanaged<void*, uint, void*, uint, int>)Slot(SlotPrepareLedInfo);
            LedGroups payload = default;
            var init = new InfoInitializeParam { MaxGroups = (uint)leds, MaxLedsPerGroup = 1, MaxLayersPerGroup = 1, LedInfo = &payload };
            Check(prepare(_mgr, InfoInitialize, &init, 0), "PrepareLedInfo(Initialize)");
            try
            {
                uint* patterns = stackalloc uint[leds];
                uint* grouping = stackalloc uint[leds * 2];
                CTColour* layer = stackalloc CTColour[leds];
                for (var i = 0; i < leds; i++)
                {
                    patterns[i] = PatternStatic;
                    grouping[i * 2] = 1; // LEDs in this group
                    grouping[i * 2 + 1] = (uint)i; // which LED
                    layer[i] = colours[i];
                }

                var fill = new InfoFillupAllParam
                {
                    NumGroups = (uint)leds, Patterns = patterns, GroupingColumns = 2, Grouping = grouping,
                    NumLayers = 1, ColourColumns = 1, Colours = layer, LedInfo = &payload,
                };
                Check(prepare(_mgr, InfoFillupAll, &fill, 0), "PrepareLedInfo(FillupAll)");

                var settings = BaseSettings();
                settings.IgnoreMasterLedState = 0;
                settings.MasterLedState = 1;
                settings.IgnoreGlobalPatternMode = 0;
                settings.GlobalPatternMode = 0;
                settings.IgnorePattern = 0;
                settings.Pattern = PatternStatic;
                settings.IgnoreColour = 0;
                settings.Colour = payload;
                Execute(&settings, "SetLedSettings");
            }
            finally
            {
                var cleanup = new InfoCleanupParam { MaxGroups = (uint)leds, LedInfo = &payload };
                prepare(_mgr, InfoCleanup, &cleanup, 0);
            }
        }
    }

    // Ends with the master LED off and the device back in global pattern mode. Without the latter Sound Blaster
    // Connect's profile switches don't take effect, as it never sets the pattern mode itself. RestoreDefault
    // isn't used: it turns on the firmware's default lighting rather than the user's profile.
    public void Dispose()
    {
        lock (_lock)
        {
            if (_mgr == null)
                return;

            if (_openLeds != 0)
            {
                _openLeds = 0;
                try
                {
                    var settings = BaseSettings();
                    settings.IgnoreMasterLedState = 0;
                    settings.MasterLedState = 0;
                    settings.IgnoreGlobalPatternMode = 0;
                    settings.GlobalPatternMode = 1;
                    Execute(&settings, "SetLedSettings(master off)");
                }
                catch
                {
                    // The device may be gone already; still close and shut down below.
                }

                ((delegate* unmanaged<void*, uint, int>)Slot(SlotClose))(_mgr, 0);
            }

            if (_initialized)
                ((delegate* unmanaged<void*, uint, int>)Slot(SlotShutdown))(_mgr, 0);
            ((delegate* unmanaged<void*, uint>)Slot(SlotRelease))(_mgr);
            _mgr = null;
        }
    }

    private void* Slot(int index) => (*(void***)_mgr)[index];

    private void Execute(SetLedSettingsParam* settings, string what) =>
        Check(((delegate* unmanaged<void*, uint, void*, uint, int>)Slot(SlotExecuteCommand))(_mgr, CommandSetLedSettings, settings, 0),
            $"ExecuteCommand({what})");

    // Everything ignored except the apply flags; Persistent = FALSE so nothing is saved to the device.
    private static SetLedSettingsParam BaseSettings() => new()
    {
        FlagForApplySettings = 3, IgnoreProfileId = 1, ProfileIdMain = 126, ProfileIdSub = 63, Persistent = 0,
        IgnoreMasterLedState = 1, MasterLedState = NotApplicable, IgnoreGlobalPatternMode = 1, GlobalPatternMode = NotApplicable,
        LedIndex = NotApplicable, IgnorePattern = 1, Pattern = NotApplicable,
        IgnorePatternDirection = 1, PatternDirection = NotApplicable,
        IgnorePatternDirectionFlag = 1, PatternDirectionFlag = NotApplicable,
        IgnorePeriodicTime = 1, PeriodicPattern = NotApplicable, PeriodicDirection = NotApplicable,
        PeriodicDirectionFlag = NotApplicable, PeriodicTimeMs = 0, PeriodicTimeCyclesPerMinute = 0,
        IgnoreColour = 1,
    };

    private static void Check(int hr, string what)
    {
        if (hr < 0)
            throw new InvalidOperationException($"Creative LED manager: {what} failed: 0x{hr:X8}");
    }

    // Struct layouts follow CTLEDCommon.h and ICTLEDMgr.h (x64: BOOL and enums are 4 bytes, pointers 8).
    private struct LedGroups
    {
        public uint NumGroups;
        public void* Handle;
    }

    private struct InfoInitializeParam
    {
        public uint MaxGroups, MaxLedsPerGroup, MaxLayersPerGroup;
        public LedGroups* LedInfo;
    }

    private struct InfoFillupAllParam
    {
        public uint NumGroups;
        public uint* Patterns;
        public uint GroupingColumns;
        public uint* Grouping;
        public uint NumLayers, ColourColumns;
        public CTColour* Colours;
        public LedGroups* LedInfo;
    }

    private struct InfoCleanupParam
    {
        public uint MaxGroups;
        public LedGroups* LedInfo;
    }

    private struct SetLedSettingsParam
    {
        public uint FlagForApplySettings, IgnoreProfileId, ProfileIdMain, ProfileIdSub, Persistent;
        public uint IgnoreMasterLedState, MasterLedState, IgnoreGlobalPatternMode, GlobalPatternMode, LedIndex;
        public uint IgnorePattern, Pattern, IgnorePatternDirection, PatternDirection, IgnorePatternDirectionFlag, PatternDirectionFlag;
        public uint IgnorePeriodicTime, PeriodicPattern, PeriodicDirection, PeriodicDirectionFlag, PeriodicTimeMs, PeriodicTimeCyclesPerMinute;
        public uint IgnoreColour;
        public LedGroups Colour;
    }
}

internal struct CTColour
{
    public byte R, G, B, A;
}

internal sealed record CreativeDevice(ushort VendorId, ushort ProductId, string Serial, string Instance, string Name, ushort LedInfoFlag, ushort Leds);
