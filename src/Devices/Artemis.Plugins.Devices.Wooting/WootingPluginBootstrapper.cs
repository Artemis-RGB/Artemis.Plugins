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
        NativeLibrary.SetDllImportResolver(typeof(WootingPluginBootstrapper).Assembly, (libraryName, assembly, searchPath) =>
        {
            if (libraryName != WootingSdk.Dll)
                return IntPtr.Zero;
            
            //replicate rgb.net's library loading for cross-platform compatibility
            string? dllPath = GetPossibleLibraryPaths().FirstOrDefault(File.Exists); 
            if (dllPath == null) 
                throw new RGBDeviceException($"Can't find the Wooting-SDK at one of the expected locations:\r\n '{string.Join("\r\n", GetPossibleLibraryPaths().Select(Path.GetFullPath))}'");
            if (!NativeLibrary.TryLoad(dllPath,assembly, searchPath, out IntPtr ret))
                throw new RGBDeviceException($"Wooting LoadLibrary failed with error code {Marshal.GetLastPInvokeError()}");

            return ret;
        });
        base.OnPluginLoaded(plugin);
    }
    
    private static IEnumerable<string> GetPossibleLibraryPaths()
    {
        IEnumerable<string> possibleLibraryPaths;

        if (OperatingSystem.IsWindows())
            possibleLibraryPaths = Environment.Is64BitProcess ? RGB.NET.Devices.Wooting.WootingDeviceProvider.PossibleX64NativePathsWindows 
                : RGB.NET.Devices.Wooting.WootingDeviceProvider.PossibleX86NativePathsWindows;
        else if (OperatingSystem.IsLinux())
            possibleLibraryPaths = RGB.NET.Devices.Wooting.WootingDeviceProvider.PossibleNativePathsLinux;
        else if (OperatingSystem.IsMacOS())
            possibleLibraryPaths = RGB.NET.Devices.Wooting.WootingDeviceProvider.PossibleNativePathsMacOS;
        else
            possibleLibraryPaths = Enumerable.Empty<string>();

        return possibleLibraryPaths.Select(Environment.ExpandEnvironmentVariables);
    }
}