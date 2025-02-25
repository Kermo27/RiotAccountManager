using System.Diagnostics;
using RiotAccountManager.MAUI.Data.Models;

namespace RiotAccountManager.MAUI.Services.RiotClientService;

public class RiotClientService : IRiotClientService
{
    private readonly IRiotClientProcessService _processService;
    private readonly IRiotClientLockfileService _lockfileService;
    private readonly IRiotClientUIAutomationService _uiAutomationService;

    public RiotClientService(
        IRiotClientProcessService processService,
        IRiotClientLockfileService lockfileService,
        IRiotClientUIAutomationService uiAutomationService
    )
    {
        _processService = processService;
        _lockfileService = lockfileService;
        _uiAutomationService = uiAutomationService;
    }

    public async Task<bool> AutoLogin(Account account)
    {
        try
        {
            KillLeagueClient();

            Thread.Sleep(100);

            var clientPath = _processService.FindRiotClientPath();
            if (clientPath == null)
                throw new FileNotFoundException("Riot Client not found");

            if (!_processService.IsClientRunning())
            {
                _processService.StartClientProcess(clientPath);
                await _lockfileService.WaitForClientReady();
            }

            await Task.Delay(5000);

            _uiAutomationService.AutomateLoginUI(account);
            return true;
        }
        catch (Exception ex)
        {
            //Logger.LogError(ex, "Błąd podczas autologowania");
            return false;
        }
    }

    private void KillLeagueClient()
    {
        try
        {
            var source = new[]
            {
                "RiotClientUxRender",
                "RiotClientUx",
                "RiotClientServices",
                "RiotClientCrashHandler",
                "LeagueCrashHandler",
                "LeagueClientUxRender",
                "LeagueClientUx",
                "LeagueClient",
            };

            var allProcessesKilled = false;

            while (!allProcessesKilled)
            {
                allProcessesKilled = true;

                foreach (var processName in source)
                {
                    var processes = Process.GetProcessesByName(processName);

                    foreach (var process in processes)
                    {
                        process.Kill();
                        allProcessesKilled = false;
                    }
                }

                if (!allProcessesKilled)
                    Thread.Sleep(1000);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}
