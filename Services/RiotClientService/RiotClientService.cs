using System.Diagnostics;
using Microsoft.Extensions.Logging;
using RiotAccountManager.MAUI.Data.Models;

namespace RiotAccountManager.MAUI.Services.RiotClientService;

public class RiotClientService : IRiotClientService
{
    private readonly IRiotClientLockfileService _lockfileService;
    private readonly ILogger<RiotClientService> _logger;
    private readonly IRiotClientProcessService _processService;
    private readonly IRiotClientUiAutomationService _uiAutomationService;

    public RiotClientService(
        IRiotClientProcessService processService,
        IRiotClientLockfileService lockfileService,
        IRiotClientUiAutomationService uiAutomationService,
        ILogger<RiotClientService> logger)
    {
        _processService = processService;
        _lockfileService = lockfileService;
        _uiAutomationService = uiAutomationService;
        _logger = logger;
    }

    public async Task<bool> AutoLogin(Account account)
    {
        try
        {
            _logger.LogInformation("Initiating auto-login process.");
            await KillLeagueClientAsync();

            //await Task.Delay(100); // Brief delay after killing client processes

            var clientPath = _processService.FindRiotClientPath();
            if (clientPath == null)
            {
                _logger.LogError("Riot Client not found.");
                throw new FileNotFoundException("Riot Client not found");
            }

            if (!_processService.IsClientRunning())
            {
                _logger.LogInformation("Riot Client is not running. Starting client process from path: {ClientPath}",
                    clientPath);
                _processService.StartClientProcess(clientPath);
                _logger.LogInformation("Waiting for Riot Client to become ready via lockfile.");
                await _lockfileService.WaitForClientReady();
            }

            _logger.LogInformation("Waiting for client stabilization.");
            await Task.Delay(5000);

            _logger.LogInformation("Automating login UI.");
            await _uiAutomationService.AutomateLoginUi(account);
            _logger.LogInformation("Auto-login process completed successfully.");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during auto-login process.");

            return false;
        }
    }

    private async Task KillLeagueClientAsync()
    {
        try
        {
            var processNames = new[]
            {
                "RiotClientUxRender",
                "RiotClientUx",
                "RiotClientServices",
                "RiotClientCrashHandler",
                "LeagueCrashHandler",
                "LeagueClientUxRender",
                "LeagueClientUx",
                "LeagueClient"
            };

            var allProcessesKilled = false;
            _logger.LogInformation("Attempting to terminate existing client processes.");

            while (!allProcessesKilled)
            {
                allProcessesKilled = true;

                foreach (var processName in processNames)
                {
                    var processes = Process.GetProcessesByName(processName);
                    foreach (var process in processes)
                        try
                        {
                            _logger.LogInformation("Terminating process {ProcessName} (ID: {ProcessId}).",
                                process.ProcessName, process.Id);
                            process.Kill();
                            allProcessesKilled = false;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to terminate process {ProcessName} (ID: {ProcessId}).",
                                process.ProcessName, process.Id);
                        }
                }

                if (!allProcessesKilled)
                {
                    _logger.LogInformation("Waiting for processes to exit...");
                    await Task.Delay(1000);
                }
            }

            _logger.LogInformation("All target client processes have been terminated.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while terminating client processes.");
        }
    }
}