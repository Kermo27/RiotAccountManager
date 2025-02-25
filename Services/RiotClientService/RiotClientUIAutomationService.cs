using RiotAccountManager.MAUI.Data.Models;
using RiotAccountManager.MAUI.Helpers;
using RiotAccountManager.MAUI.Services.EncryptionService;

namespace RiotAccountManager.MAUI.Services.RiotClientService;

public class RiotClientUIAutomationService : IRiotClientUIAutomationService
{
    private readonly IEncryptionService _encryptionService;

    public RiotClientUIAutomationService(IEncryptionService encryptionService)
    {
        _encryptionService = encryptionService;
    }

    public void AutomateLoginUI(Account account)
    {
        // Pobierz dane użytkownika
        var username = account.Username;
        var password = account.GetDecryptedPassword(_encryptionService);

        // Znajdź okno Riot Clienta (upewnij się, że nazwa odpowiada tytułowi okna)
        IntPtr hWnd = Win32Helper.FindWindow(null, "Riot Client");
        if (hWnd == IntPtr.Zero)
            throw new Exception("Nie znaleziono okna Riot Clienta.");

        // Ustaw okno jako aktywne
        Win32Helper.SetForegroundWindow(hWnd);
        Thread.Sleep(100);

        // Wklej login
        KeyboardHelper.PasteText(username);
        Thread.Sleep(100);

        // Naciśnij TAB, aby przejść do pola hasła
        KeyboardHelper.SendVirtualKey(VirtualKey.VK_TAB);
        Thread.Sleep(100);

        // Wklej hasło
        KeyboardHelper.PasteText(password);
        Thread.Sleep(100);

        // Naciśnij ENTER, aby zatwierdzić logowanie
        KeyboardHelper.SendVirtualKey(VirtualKey.VK_RETURN);
        Thread.Sleep(100);
    }
}
