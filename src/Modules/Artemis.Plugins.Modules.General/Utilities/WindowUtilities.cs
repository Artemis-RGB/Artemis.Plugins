using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Artemis.Plugins.Modules.General.Utilities
{
    public static class WindowUtilities
    {
        public static int GetActiveProcessId()
        {
            // Get foreground window handle
            IntPtr hWnd = GetForegroundWindow();

            GetWindowThreadProcessId(hWnd, out uint processId);
            return (int) processId;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        public static string GetActiveWindowTitle()
        {
            // Get foreground window handle
            IntPtr hWnd = GetForegroundWindow(); 

            int length = GetWindowTextLength(hWnd) + 1;
            StringBuilder title = new StringBuilder(length);
            GetWindowText(hWnd, title, length);
            return title.ToString();
        }
        
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
    }
}