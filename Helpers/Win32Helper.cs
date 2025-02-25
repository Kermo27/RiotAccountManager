using System.Runtime.InteropServices;

namespace RiotAccountManager.MAUI.Helpers;

public static class Win32Helper
{
    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool SetForegroundWindow(IntPtr hWnd);
}
