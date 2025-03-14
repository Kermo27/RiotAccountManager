namespace RiotAccountManager.MAUI.Services.ApplicationUpdateService;

public interface IApplicationUpdateService
{
    Task<Version> CheckForUpdates();
    Task DownloadUpdate(string downloadPath);
}