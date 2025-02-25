using System.Diagnostics;
using Microsoft.Win32;

namespace RiotAccountManager.MAUI.Services.RiotClientService;

public class RiotClientProcessService : IRiotClientProcessService
{
    private const string ClientName = "RiotClientServices.exe";
    private const string DefaultInstallPath = @"C:\Riot Games\Riot Client\";

    public string FindRiotClientPath()
    {
        const string registryKey =
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Riot Game riot_league_of_legends.live";
        using var key = Registry.LocalMachine.OpenSubKey(registryKey);

        if (key != null)
        {
            var installLocation = key.GetValue("InstallLocation")?.ToString();
            if (!string.IsNullOrEmpty(installLocation))
            {
                var fullPath = Path.Combine(installLocation, "Riot Client", ClientName);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }
        }

        var commonPaths = new[]
        {
            DefaultInstallPath,
            @"C:\Program Files\Riot Games\Riot Client\",
            @"C:\Program Files (x86)\Riot Games\Riot Client\",
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                "Riot Games",
                "Riot Client"
            ),
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                "Riot Games",
                "Riot Client"
            ),
        };

        foreach (var path in commonPaths)
        {
            var fullPath = Path.Combine(path, ClientName);
            if (File.Exists(fullPath))
            {
                return fullPath;
            }
        }

        var process = Process.GetProcessesByName("RiotClientServices").FirstOrDefault();
        if (process != null)
        {
            try
            {
                return process.MainModule?.FileName;
            }
            catch
            {
                throw new Exception("Failed to get Riot Client path.");
            }
        }

        return null;
    }

    public bool IsClientRunning()
    {
        try
        {
            var processes = Process.GetProcessesByName("RiotClientServices");

            return processes.Length > 0;
        }
        catch
        {
            return false;
        }
    }

    public void StartClientProcess(string clientPath)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = clientPath,
                Arguments = "-- --allow-multiple-clients", // Zezwól na wiele instancji
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
            };

            var process = new Process { StartInfo = startInfo };

            if (!process.Start())
            {
                throw new InvalidOperationException("Failed to start Riot Client process");
            }
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error starting Riot Client", ex);
        }
    }
}
