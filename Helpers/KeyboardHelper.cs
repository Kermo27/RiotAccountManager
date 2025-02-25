using System.Runtime.InteropServices;

namespace RiotAccountManager.MAUI.Helpers;

public static class KeyboardHelper
{
    private const int KEYEVENTF_KEYUP = 0x0002;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);

    public static void PasteText(string text)
    {
        // Ustaw tekst w schowku
        ClipboardHelper.SetText(text);

        // Symulacja: naciśnięcie Ctrl+V
        keybd_event(VirtualKey.VK_CONTROL, 0, 0, IntPtr.Zero);
        keybd_event(VirtualKey.VK_V, 0, 0, IntPtr.Zero);
        Thread.Sleep(50);
        keybd_event(VirtualKey.VK_V, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
        keybd_event(VirtualKey.VK_CONTROL, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
    }

    public static void SendVirtualKey(byte vk)
    {
        keybd_event(vk, 0, 0, IntPtr.Zero);
        Thread.Sleep(50);
        keybd_event(vk, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
    }
}

public static class VirtualKey
{
    public const byte VK_TAB = 0x09;
    public const byte VK_RETURN = 0x0D;
    public const byte VK_CONTROL = 0x11;
    public const byte VK_V = 0x56;
}
