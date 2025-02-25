using System.Diagnostics;
using Microsoft.Win32;
using RiotAccountManager.MAUI.Data.Models;
using RiotAccountManager.MAUI.Helpers;

namespace RiotAccountManager.MAUI.Services;

public class RiotClientService
{
    private const string ClientName = "RiotClientServices.exe";
    private const string DefaultInstallPath = @"C:\Riot Games\Riot Client\";

    private readonly EncryptionService _encryptionService;

    public RiotClientService(EncryptionService encryptionService)
    {
        _encryptionService = encryptionService;
    }

    public async Task<bool> AutoLogin(Account account)
    {
        try
        {
            var clientPath = FindRiotClientPath();
            if (clientPath == null)
                throw new FileNotFoundException("Riot Client not found");

            if (!IsClientRunning())
            {
                StartClientProcess(clientPath);
                await WaitForClientReady();
            }

            // Dodatkowe opóźnienie, aby mieć pewność, że okno jest gotowe.
            await Task.Delay(5000);

            // Wywołanie automatyzacji logowania przez kopiuj-wklej
            AutomateLoginUI(account);
            return true;
        }
        catch (Exception ex)
        {
            //Logger.LogError(ex, "Błąd podczas autologowania");
            return false;
        }
    }

    private void AutomateLoginUI(Account account)
    {
        // Pobierz dane użytkownika
        string username = account.Username;
        string password = account.GetDecryptedPassword(_encryptionService);

        // Znajdź okno Riot Clienta (upewnij się, że nazwa odpowiada tytułowi okna)
        IntPtr hWnd = Win32Helper.FindWindow(null, "Riot Client");
        if (hWnd == IntPtr.Zero)
            throw new Exception("Nie znaleziono okna Riot Clienta.");

        // Ustaw okno jako aktywne
        Win32Helper.SetForegroundWindow(hWnd);
        Thread.Sleep(100);

        // Wklej login
        KeyboardHelper.PasteText(username);
        Thread.Sleep(100);

        // Naciśnij TAB, aby przejść do pola hasła
        KeyboardHelper.SendVirtualKey(VirtualKey.VK_TAB);
        Thread.Sleep(100);

        // Wklej hasło
        KeyboardHelper.PasteText(password);
        Thread.Sleep(100);

        // Naciśnij ENTER, aby zatwierdzić logowanie
        KeyboardHelper.SendVirtualKey(VirtualKey.VK_RETURN);
        Thread.Sleep(100);
    }

    private async Task WaitForClientReady()
    {
        const int maxAttempts = 30;
        const int delayMs = 1000;
        var lockfilePath = GetLockfilePath();

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            try
            {
                if (File.Exists(lockfilePath))
                {
                    using var fileStream = new FileStream(
                        lockfilePath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.ReadWrite
                    );

                    using var reader = new StreamReader(fileStream);
                    var content = await reader.ReadToEndAsync();

                    if (content.Split(':').Length >= 4)
                    {
                        return;
                    }
                }
            }
            catch (IOException)
            {
                throw new IOException("Błąd odczytu pliku lockfile");
            }

            await Task.Delay(delayMs);
        }

        throw new TimeoutException("Riot Client nie odpowiada");
    }

    private string? GetLockfilePath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Riot Games",
            "Riot Client",
            "Config",
            "lockfile"
        );
    }

    private void StartClientProcess(string clientPath)
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

    private bool IsClientRunning()
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

    private string FindRiotClientPath()
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
}
