using System;
using System.Runtime.InteropServices;

namespace RiotAccountManager.MAUI.Helpers;

public static class ClipboardHelper
{
    private const uint CfUnicodeText = 13;
    private const uint GmemMoveable = 0x0002;

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

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GlobalFree(IntPtr hMem);

    public static bool SetText(string text)
    {
        if (!OpenClipboard(IntPtr.Zero))
            return false;

        try
        {
            if (!EmptyClipboard())
                return false;

            // Calculate the number of bytes required (including null terminator)
            var byteCount = (text.Length + 1) * 2;
            var hGlobal = GlobalAlloc(GmemMoveable, (UIntPtr)byteCount);
            if (hGlobal == IntPtr.Zero)
                return false;

            var target = GlobalLock(hGlobal);
            if (target == IntPtr.Zero)
            {
                GlobalFree(hGlobal);
                return false;
            }

            // Copy string to unmanaged memory
            var chars = text.ToCharArray();
            Marshal.Copy(chars, 0, target, text.Length);
            // Add null terminator
            Marshal.WriteInt16(target, text.Length * 2, 0);

            GlobalUnlock(hGlobal);

            // Set the clipboard data
            var result = SetClipboardData(CfUnicodeText, hGlobal);
            if (result == IntPtr.Zero)
            {
                // Free the memory if the clipboard operation failed
                GlobalFree(hGlobal);
                return false;
            }

            // On success, the system owns the memory so do not free hGlobal.
            return true;
        }
        finally
        {
            CloseClipboard();
        }
    }
}
