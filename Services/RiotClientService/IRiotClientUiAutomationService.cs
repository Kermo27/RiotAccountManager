using RiotAccountManager.MAUI.Data.Models;

namespace RiotAccountManager.MAUI.Services.RiotClientService;

public interface IRiotClientUiAutomationService
{
    Task AutomateLoginUi(Account account);
}
