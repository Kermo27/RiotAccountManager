using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace RiotAccountManager.MAUI.Helpers
{
    public static class KeyboardHelper
    {
        private const int KEYEVENTF_KEYUP = 0x0002;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);

        public static void PasteText(string text)
        {
            if (ClipboardHelper.SetText(text))
            {
                // Simulate Ctrl+V to paste the text
                PressKey(VirtualKey.VkControl);
                PressKey(VirtualKey.VkV);
                Thread.Sleep(50);
                ReleaseKey(VirtualKey.VkV);
                ReleaseKey(VirtualKey.VkControl);
            }
            else
            {
                throw new InvalidOperationException("Failed to set text to clipboard.");
            }
        }

        public static void SendVirtualKey(byte vk)
        {
            PressKey(vk);
            Thread.Sleep(50);
            ReleaseKey(vk);
        }

        private static void PressKey(byte vk)
        {
            keybd_event(vk, 0, 0, IntPtr.Zero);
        }

        private static void ReleaseKey(byte vk)
        {
            keybd_event(vk, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
        }
    }

    public static class VirtualKey
    {
        public const byte VkTab = 0x09;
        public const byte VkReturn = 0x0D;
        public const byte VkControl = 0x11;
        public const byte VkV = 0x56;
    }
}