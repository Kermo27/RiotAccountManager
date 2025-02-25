using System.Runtime.InteropServices;

namespace RiotAccountManager.MAUI.Helpers;

public static class ClipboardHelper
{
    private const uint CF_UNICODETEXT = 13;
    private const uint GMEM_MOVEABLE = 0x0002;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool OpenClipboard(IntPtr hWndNewOwner);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool EmptyClipboard();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool CloseClipboard();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GlobalLock(IntPtr hMem);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GlobalUnlock(IntPtr hMem);

    public static bool SetText(string text)
    {
        if (!OpenClipboard(IntPtr.Zero))
            return false;

        try
        {
            EmptyClipboard();
            // Oblicz rozmiar pamięci: (ilość znaków + 1) * 2 bajty (Unicode)
            var bytes = (text.Length + 1) * 2;
            IntPtr hGlobal = GlobalAlloc(GMEM_MOVEABLE, (UIntPtr)bytes);
            if (hGlobal == IntPtr.Zero)
                return false;

            IntPtr target = GlobalLock(hGlobal);
            if (target == IntPtr.Zero)
                return false;

            // Skopiuj tekst do zaalokowanej pamięci
            char[] chars = text.ToCharArray();
            Marshal.Copy(chars, 0, target, text.Length);
            // Zapisz znak null jako terminator
            Marshal.WriteInt16(target, text.Length * 2, 0);
            GlobalUnlock(hGlobal);

            if (SetClipboardData(CF_UNICODETEXT, hGlobal) == IntPtr.Zero)
                return false;
        }
        finally
        {
            CloseClipboard();
        }
        return true;
    }
}
