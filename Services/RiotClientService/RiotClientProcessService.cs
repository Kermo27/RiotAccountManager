using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace RiotAccountManager.MAUI.Services.RiotClientService;

public class RiotClientProcessService : IRiotClientProcessService
{
    private const string ClientName = "RiotClientServices.exe";
    private const string DefaultInstallPath = @"C:\Riot Games\Riot Client\";
    private readonly ILogger<RiotClientProcessService> _logger;

    public RiotClientProcessService(ILogger<RiotClientProcessService> logger)
    {
        _logger = logger;
    }

    public string? FindRiotClientPath()
    {
        _logger.LogInformation("Attempting to find Riot Client path via registry.");
        const string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Riot Game riot_league_of_legends.live";
        using var key = Registry.LocalMachine.OpenSubKey(registryKey);
        
        if (key != null)
        {
            var installLocation = key.GetValue("InstallLocation")?.ToString();
            if (!string.IsNullOrEmpty(installLocation))
            {
                var fullPath = Path.Combine(installLocation, "Riot Client", ClientName);
                if (File.Exists(fullPath))
                {
                    _logger.LogInformation("Riot Client found via registry at path: {FullPath}", fullPath);
                    
                    return fullPath;
                }
            }
            else
            {
                _logger.LogDebug("Registry key found, but InstallLocation value is null or empty.");
            }
        }
        else
        {
            _logger.LogDebug("Registry key not found: {RegistryKey}", registryKey);
        }

        _logger.LogInformation("Attempting to find Riot Client path via common installation paths.");
        var commonPaths = new[]
        {
            DefaultInstallPath,
            @"C:\Program Files\Riot Games\Riot Client\",
            @"C:\Program Files (x86)\Riot Games\Riot Client\",
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Riot Games", "Riot Client"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Riot Games", "Riot Client"),
        };

        foreach (var path in commonPaths)
        {
            var fullPath = Path.Combine(path, ClientName);
            _logger.LogDebug("Checking path: {FullPath}", fullPath);
            if (File.Exists(fullPath))
            {
                _logger.LogInformation("Riot Client found at path: {FullPath}", fullPath);
                
                return fullPath;
            }
        }

        _logger.LogInformation("Attempting to find Riot Client via running process.");
        var process = Process.GetProcessesByName("RiotClientServices").FirstOrDefault();
        if (process != null)
        {
            try
            {
                var processPath = process.MainModule?.FileName;
                if (!string.IsNullOrEmpty(processPath))
                {
                    _logger.LogInformation("Riot Client found via running process at path: {ProcessPath}", processPath);
                    
                    return processPath;
                }
                else
                {
                    _logger.LogWarning("Riot Client process found but main module file path is null.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve Riot Client path from running process.");
                throw new Exception("Failed to get Riot Client path from running process.", ex);
            }
        }

        _logger.LogWarning("Riot Client path not found.");
        
        return null;
    }

    public bool IsClientRunning()
    {
        try
        {
            var processes = Process.GetProcessesByName("RiotClientServices");
            var isRunning = processes.Any();
            _logger.LogInformation("Riot Client running status: {IsRunning}", isRunning);
            
            return isRunning;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while checking if Riot Client is running.");
            
            return false;
        }
    }

    public void StartClientProcess(string clientPath)
    {
        _logger.LogInformation("Attempting to start Riot Client process from path: {ClientPath}", clientPath);
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = clientPath,
                Arguments = "-- --allow-multiple-clients",
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
            };

            var process = new Process { StartInfo = startInfo };

            if (!process.Start())
            {
                _logger.LogError("Failed to start Riot Client process.");
                throw new InvalidOperationException("Failed to start Riot Client process");
            }
            _logger.LogInformation("Riot Client process started successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting Riot Client process.");
            throw new ApplicationException("Error starting Riot Client", ex);
        }
    }
}
