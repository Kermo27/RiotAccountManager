using Microsoft.Extensions.Logging;
using RiotAccountManager.MAUI.Data.Models;
using RiotAccountManager.MAUI.Helpers;
using RiotAccountManager.MAUI.Services.EncryptionService;
using WindowsInput;

namespace RiotAccountManager.MAUI.Services.RiotClientService;

public class RiotClientUiAutomationService : IRiotClientUiAutomationService
{
    private readonly IEncryptionService _encryptionService;
    private readonly IInputSimulator _inputSimulator;
    private readonly ILogger<RiotClientUiAutomationService> _logger;

    public RiotClientUiAutomationService(
        IEncryptionService encryptionService,
        ILogger<RiotClientUiAutomationService> logger,
        IInputSimulator inputSimulator)
    {
        _encryptionService = encryptionService;
        _logger = logger;
        _inputSimulator = inputSimulator;
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

        _logger.LogInformation("Pasting username.");
        await PasteTextAsync(username);
        await Task.Delay(100);

        _logger.LogInformation("Sending Tab key to switch focus.");
        _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.TAB);

        _logger.LogInformation("Pasting password.");
        await PasteTextAsync(password);

        _logger.LogInformation("Sending Return key to submit login.");
        _inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);

        _logger.LogInformation("UI automation for auto-login completed successfully.");
    }

    private async Task PasteTextAsync(string text)
    {
        await Clipboard.Default.SetTextAsync(text);

        _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
    }
}