using RiotAccountManager.MAUI.Data.Models;

namespace RiotAccountManager.MAUI.Services.RiotClientService;

public interface IRiotClientService
{
    Task<bool> AutoLogin(Account account);
}
