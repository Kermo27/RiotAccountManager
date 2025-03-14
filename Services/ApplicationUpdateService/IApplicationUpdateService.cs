namespace RiotAccountManager.MAUI.Services.ApplicationUpdateService;

public interface IApplicationUpdateService
{
    Task<bool> CheckForUpdates();
    Task DownloadAndApplyUpdate();
}