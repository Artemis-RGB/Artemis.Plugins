using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Artemis.Core;
using Artemis.Plugins.Devices.Wooting.Services.ProfileService;
using RGB.NET.Core;

namespace Artemis.Plugins.Devices.Wooting;

public class WootingPluginBootstrapper : PluginBootstrapper
{
    public override void OnPluginLoaded(Plugin plugin)
    {
        string dllPath = GetLibraryPath(plugin);
        
        NativeLibrary.SetDllImportResolver(typeof(WootingPluginBootstrapper).Assembly, (libraryName, assembly, searchPath) =>
        {
            if (libraryName != WootingSdk.Dll)
                return IntPtr.Zero;
            if (!File.Exists(dllPath)) 
                throw new RGBDeviceException($"Can't find the Wooting-SDK");
            if (!NativeLibrary.TryLoad(dllPath, assembly, searchPath, out IntPtr ret))
                throw new RGBDeviceException($"Wooting LoadLibrary failed with error code {Marshal.GetLastPInvokeError()}");

            return ret;
        });
        base.OnPluginLoaded(plugin);
    }
    
    private static string GetLibraryPath(Plugin plugin)
    {
        if (OperatingSystem.IsWindows())
            return Path.Combine(plugin.Directory.FullName, "x64", "wooting-rgb-sdk64.dll");
        
        if (OperatingSystem.IsLinux())
            return Path.Combine(plugin.Directory.FullName, "x64", "x64/libwooting-rgb-sdk.so");
        
        if (OperatingSystem.IsMacOS())
            return Path.Combine(plugin.Directory.FullName, "x64", "libwooting-rgb-sdk.dylib");

        throw new PlatformNotSupportedException();
    }
}