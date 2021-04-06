using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Artemis.Plugins.Modules.General.Utilities
{
    public static class Performance
    {
        #region Properties & Fields

        private static readonly PerformanceCounter _cpuPerformanceCounter = new("Processor", "% Processor Time", "_Total");

        #endregion

        #region DLLImports

        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

        [StructLayout(LayoutKind.Sequential)]
        public struct PerformanceInformation
        {
            public int Size;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonPaged;
            public IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }

        #endregion

        #region Methods

        public static float GetCpuUsage()
        {
            return _cpuPerformanceCounter.NextValue();
        }

        public static long GetPhysicalAvailableMemoryInMiB()
        {
            PerformanceInformation pi = new();
            return GetPerformanceInfo(out pi, Marshal.SizeOf(pi)) ? Convert.ToInt64(pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576) : 0;
        }

        public static long GetTotalMemoryInMiB()
        {
            PerformanceInformation pi = new();
            return GetPerformanceInfo(out pi, Marshal.SizeOf(pi)) ? Convert.ToInt64(pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 1048576) : 0;
        }

        #endregion
    }
}