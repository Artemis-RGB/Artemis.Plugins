using Microsoft.Win32;

namespace Artemis.Plugins.Devices.Logitech.Prerequisites
{
    internal static class LogitechSoftwareChecker
    {
        internal static bool IsLgsInstalled() => DoesRegistrySubKeyExist(REGISTRY_LGS);

        internal static bool IsLghubInstalled() => DoesRegistrySubKeyExist(REGISTRY_LGHUB);

        internal static bool DoesRegistrySubKeyExist(string subkey)
        {
            using RegistryKey installedPrograms = Registry.LocalMachine.OpenSubKey(REGISTRY_INSTALLED_SOFTWARE);

            using RegistryKey subKey = installedPrograms.OpenSubKey(subkey);

            return subkey != null;
        }

        private const string REGISTRY_INSTALLED_SOFTWARE = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        private const string REGISTRY_LGHUB = "{521c89be-637f-4274-a840-baaf7460c2b2}";
        private const string REGISTRY_LGS = "Logitech Gaming Software";
    }
}