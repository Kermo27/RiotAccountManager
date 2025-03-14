using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using RiotAccountManager.MAUI.Data.Models;

namespace RiotAccountManager.MAUI.Services.ApplicationUpdateService;

public class ApplicationUpdateService : IApplicationUpdateService
{
    private const string RepoUrl = "https://api.github.com/repos/Kermo27/RiotAccountManager/releases/latest";
    private GitHubRelease _latestRelease;

    public async Task<Version> CheckForUpdates()
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("RiotAccountManager", "1.0"));

        var response = await client.GetStringAsync(RepoUrl);
        var release = JsonSerializer.Deserialize<GitHubRelease>(response);

        var latestVersion = new Version(_latestRelease.TagName.TrimStart('v'));
        var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

        return latestVersion > currentVersion ? latestVersion : null;
    }

    public async Task DownloadUpdate(string downloadPath)
    {
        if (_latestRelease?.Assets == null || !_latestRelease.Assets.Any())
        {
            throw new InvalidOperationException("No files to download.");
        }

        var assetUrl = _latestRelease.Assets[0].BrowserDownloadUrl;

        using var httpClient = new HttpClient();
        var stream = await httpClient.GetStreamAsync(assetUrl);

        using var fileStream = new FileStream(downloadPath, FileMode.Create);
        await stream.CopyToAsync(fileStream);
    }
}