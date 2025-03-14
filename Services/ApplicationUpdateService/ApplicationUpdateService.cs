using System.Diagnostics;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Text.Json;
using RiotAccountManager.MAUI.Data.Models;

namespace RiotAccountManager.MAUI.Services.ApplicationUpdateService;

public class ApplicationUpdateService : IApplicationUpdateService, IDisposable
{
    private const string RepoUrl = "https://api.github.com/repos/Kermo27/RiotAccountManager/releases/latest";
    private const string UpdateFileName = "RiotAccountManager.zip";
    private readonly HttpClient _client;
    private GitHubRelease _release;

    public ApplicationUpdateService()
    {
        _client = new HttpClient();
        _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("RiotManager", "1.0"));
    }

    public async Task<bool> CheckForUpdates()
    {
        var response = await _client.GetStringAsync(RepoUrl);
        _release = JsonSerializer.Deserialize<GitHubRelease>(response);

        var latestVersion = new Version(_release.TagName.TrimStart('v'));
        var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

        return latestVersion > currentVersion;
    }

    public async Task DownloadAndApplyUpdate()
    {
        var zipAsset = _release.Assets.FirstOrDefault(a => a.Name.EndsWith(".zip"));
        if (zipAsset == null)
        {
            throw new Exception("Brak pliku ZIP w release");
        }

        var tempPath = Path.Combine(Path.GetTempPath(), UpdateFileName);

        using var response = await _client.GetAsync(zipAsset.BrowserDownloadUrl);
        using var fs = new FileStream(tempPath, FileMode.Create);
        await response.Content.CopyToAsync(fs);

        await VerifyChecksum(tempPath);

        await ApplyUpdate(tempPath);
    }

    public void Dispose()
    {
        _client?.Dispose();
    }

    private async Task VerifyChecksum(string filePath)
    {
        var checksumAsset = _release.Assets.FirstOrDefault(a => a.Name == "checksum.sha256");
        if (checksumAsset == null)
        {
            throw new Exception("Brak pliku checksum");
        }

        var expectedHash = await _client.GetStringAsync(checksumAsset.BrowserDownloadUrl);
        using var sha = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var actualHash = BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "");

        if (actualHash != expectedHash)
        {
            throw new SecurityException("Nieprawidłowy checksum pliku!");
        }
    }

    private async Task ApplyUpdate(string zipPath)
    {
        var appPath = AppDomain.CurrentDomain.BaseDirectory;
        var updaterScript = @$"
            @echo off
            timeout /t 3 /nobreak >nul
            tar -xf ""{zipPath}"" -C ""{appPath}""
            del ""{zipPath}""
            start """" ""{Process.GetCurrentProcess().MainModule.FileName}""
            del ""%~f0""";

        File.WriteAllText("update.bat", updaterScript);

        Process.Start(new ProcessStartInfo
        {
            FileName = "update.bat",
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = true
        });

        Environment.Exit(0);
    }
}