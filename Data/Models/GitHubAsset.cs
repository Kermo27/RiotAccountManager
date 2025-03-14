using System.Text.Json.Serialization;

namespace RiotAccountManager.MAUI.Data.Models;

public record GitHubAsset(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("browser_download_url")]
    string BrowserDownloadUrl);