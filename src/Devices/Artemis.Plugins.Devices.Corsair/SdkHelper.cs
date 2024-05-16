using System.Diagnostics;
using System.IO;
using System.Threading;
using Artemis.Core;
using Microsoft.Win32;
using Serilog;

namespace Artemis.Plugins.Devices.Corsair;

public static class SdkHelper
{
    private const string ICUE_REGISTRY_KEY = @"HKEY_LOCAL_MACHINE\SOFTWARE\Corsair\Corsair Utility Engine";
    
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
        
        logger.Information("Waiting 8 sec for the iCUE SDK to become available...");
        Thread.Sleep(8000);
    }
    
    private static string? GetICuePath()
    {
        // The UninstallString is the path to the uninstaller, we need to extract the path to the executable
        if (Registry.GetValue(ICUE_REGISTRY_KEY, "InstallLocation", null) is not string installLocation)
            return null;

        return installLocation;
    }
}