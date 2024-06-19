using System.Diagnostics;
using System.IO;
using System.Threading;
using Artemis.Core;
using Microsoft.Win32;
using Serilog;

namespace Artemis.Plugins.Devices.Corsair;

public static class SdkHelper
{
    private static string[] _registryKeys =
    [
        @"HKEY_LOCAL_MACHINE\SOFTWARE\Corsair\Corsair Utility Engine", // iCUE 4
        @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{A9B0B2D7-8C59-4413-A2FB-99EDBE65A608}" // iCUE 5
    ];

    public static void EnsureSdkAvailable(ILogger logger)
    {
        // If iCUE is running, the SDK is available
        if (Process.GetProcessesByName("iCUE").Length > 0)
            return;

        string? icuePath = GetICuePath();
        if (icuePath == null)
            throw new ArtemisPluginException("iCUE does not seem to be installed.");

        string path = Path.Combine(icuePath, "iCUE.exe");
        logger.Information("Starting iCUE at {Path}", path);

        ProcessStartInfo startInfo = new() {FileName = Path.Combine(icuePath, "iCUE.exe"), Arguments = "--autorun", UseShellExecute = true};
        Process.Start(startInfo);

        logger.Information("Waiting 10 sec for the iCUE SDK to become available...");
        Thread.Sleep(10000);
    }

    private static string? GetICuePath()
    {
        foreach (string registryKey in _registryKeys)
        {
            // The UninstallString is the path to the uninstaller, we need to extract the path to the executable
            if (Registry.GetValue(registryKey, "InstallLocation", null) is string installLocation)
                return installLocation;
        }

        return null;
    }
}