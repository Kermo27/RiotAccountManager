using Microsoft.Extensions.Logging;
using RiotAccountManager.MAUI.Data.Models;
using RiotAccountManager.MAUI.Helpers;
using RiotAccountManager.MAUI.Services.EncryptionService;

namespace RiotAccountManager.MAUI.Services.RiotClientService;

public class RiotClientUiAutomationService : IRiotClientUiAutomationService
{
    private readonly IEncryptionService _encryptionService;
    private readonly ILogger<RiotClientUiAutomationService> _logger;

    public RiotClientUiAutomationService(
        IEncryptionService encryptionService, 
        ILogger<RiotClientUiAutomationService> logger)
    {
        _encryptionService = encryptionService;
        _logger = logger;
    }

    public async Task AutomateLoginUi(Account account)
    {
        _logger.LogInformation("Starting UI automation for auto-login.");

        var username = account.Username;
        var password = account.GetDecryptedPassword(_encryptionService);

        var hWnd = Win32Helper.FindWindow(null, "Riot Client");
        if (hWnd == IntPtr.Zero)
        {
            _logger.LogError("Riot Client window not found.");
            throw new Exception("Riot Client window not found.");
        }

        _logger.LogInformation("Bringing Riot Client window to the foreground.");
        Win32Helper.SetForegroundWindow(hWnd);
        await Task.Delay(100);

        _logger.LogInformation("Pasting username.");
        KeyboardHelper.PasteText(username);
        await Task.Delay(100);

        _logger.LogInformation("Sending Tab key to switch focus.");
        KeyboardHelper.SendVirtualKey(VirtualKey.VkTab);
        await Task.Delay(100);

        _logger.LogInformation("Pasting password.");
        KeyboardHelper.PasteText(password);
        await Task.Delay(100);

        _logger.LogInformation("Sending Return key to submit login.");
        KeyboardHelper.SendVirtualKey(VirtualKey.VkReturn);
        await Task.Delay(100);

        _logger.LogInformation("UI automation for auto-login completed successfully.");
    }
}
