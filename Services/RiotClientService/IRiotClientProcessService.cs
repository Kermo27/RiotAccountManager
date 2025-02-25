namespace RiotAccountManager.MAUI.Services.RiotClientService;

public interface IRiotClientProcessService
{
    string FindRiotClientPath();
    bool IsClientRunning();
    void StartClientProcess(string clientPath);
}
