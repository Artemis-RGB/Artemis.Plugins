using Microsoft.Win32;

namespace Artemis.Plugins.Devices.Logitech.Prerequisites
{
    internal static class LogitechSoftwareChecker
    {
        private const string REGISTRY_INSTALLED_SOFTWARE = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        private const string REGISTRY_LGHUB = "{521c89be-637f-4274-a840-baaf7460c2b2}";
        private const string REGISTRY_LGS = "Logitech Gaming Software";

        private const string REGISTRY_LOGI_SDK_DLL_PATH = @"SOFTWARE\Classes\CLSID\{a6519e67-7632-4375-afdf-caa889744403}\ServerBinary";

        private static bool DoesRegistrySubKeyExist(string subkey)
        {
            using RegistryKey installedPrograms = Registry.LocalMachine.OpenSubKey(REGISTRY_INSTALLED_SOFTWARE);

            using RegistryKey subKey = installedPrograms.OpenSubKey(subkey);

            return subkey != null;
        }

        internal static bool IsLgsInstalled() => DoesRegistrySubKeyExist(REGISTRY_LGS);

        internal static bool IsLghubInstalled() => DoesRegistrySubKeyExist(REGISTRY_LGHUB);

        internal static string GetLogitechDllPath()
        {
            using RegistryKey key = Registry.LocalMachine.OpenSubKey(REGISTRY_LOGI_SDK_DLL_PATH);

            return key?.GetValue(null)?.ToString();
        }
    }
}