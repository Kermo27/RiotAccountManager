using System.Runtime.InteropServices;

namespace RiotAccountManager.MAUI.Helpers;

public static class Win32Helper
{
    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    public const int SW_RESTORE = 9;

    public static bool IsWindowMinimized(IntPtr hWnd)
    {
        return IsIconic(hWnd);
    }

    public static void RestoreWindow(IntPtr hWnd)
    {
        ShowWindow(hWnd, SW_RESTORE);
    }

    [DllImport("user32.dll")]
    private static extern bool IsIconic(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
}
