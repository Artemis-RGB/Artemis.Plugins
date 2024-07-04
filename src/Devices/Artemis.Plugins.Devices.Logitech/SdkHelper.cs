using System.Diagnostics;
using System.IO;
using System.Threading;
using Artemis.Core;
using Microsoft.Win32;
using Serilog;

namespace Artemis.Plugins.Devices.Logitech;

public static class SdkHelper
{
    private const string GHUB_REGISTRY_KEY = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{521c89be-637f-4274-a840-baaf7460c2b2}";
    private const string LGS_REGISTRY_KEY = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Logitech Gaming Software";
    
    public static void EnsureSdkAvailable(ILogger logger)
    {
        // If GHub or LGS are running, the SDK is available
        if (Process.GetProcessesByName("lghub_agent").Length != 0 || Process.GetProcessesByName("lgs").Length != 0 || Process.GetProcessesByName("LCore").Length != 0)
            return;

        // Neither is running, check if either is installed
        string? ghubPath = GetGHubPath();
        string? lgsPath = GetLgsPath();
        
        if (ghubPath == null && lgsPath == null)
            throw new ArtemisPluginException("Neither GHub nor LGS seem to be installed, the Logitech SDK is required to use this plugin");

        if (ghubPath != null)
        {
            string path = Path.Combine(ghubPath, "system_tray", "lghub_system_tray.exe");
            logger.Information("Starting Logitech GHub at {Path}", path);

            // Start GHub
            ProcessStartInfo startInfo = new() {FileName = path, Arguments = "--minimized", UseShellExecute = true};
            Process.Start(startInfo);
            
            // Wait for GHub, slowpoke
            logger.Information("Waiting 14 sec for the Logitech SDK to become available...");
            Thread.Sleep(14000);
        }
        else if (lgsPath != null)
        {
            string path = Path.Combine(lgsPath, "LCore.exe");
            logger.Information("Starting LGS at {Path}", path);

            // Start LGS
            ProcessStartInfo startInfo = new() {FileName = path, Arguments = "/minimized", UseShellExecute = true};
            Process.Start(startInfo);
            
            // Wait for LGS
            logger.Information("Waiting 4 sec for the Logitech SDK to become available...");
            Thread.Sleep(4000);
        }
    }

    private static string? GetGHubPath()
    {
        // The DisplayIcon is the path to lghub.exe, we need to extract the path to the executable
        if (Registry.GetValue(GHUB_REGISTRY_KEY, "DisplayIcon", null) is not string displayIcon)
            return null;

        return Path.GetDirectoryName(displayIcon.Trim('"'));
    }

    private static string? GetLgsPath()
    {
        // The DisplayIcon is the path to LCore.exe, we need to extract the path to the executable
        if (Registry.GetValue(LGS_REGISTRY_KEY, "DisplayIcon", null) is not string displayIcon)
            return null;

        return Path.GetDirectoryName(displayIcon.Trim('"'));
    }
}