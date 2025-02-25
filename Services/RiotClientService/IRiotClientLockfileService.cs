namespace RiotAccountManager.MAUI.Services.RiotClientService;

public interface IRiotClientLockfileService
{
    Task WaitForClientReady();
}
