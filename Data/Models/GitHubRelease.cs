using System.Text.Json.Serialization;

namespace RiotAccountManager.MAUI.Data.Models;

public record GitHubRelease(
    [property: JsonPropertyName("tag_name")]
    string TagName,
    [property: JsonPropertyName("assets")] List<GitHubAsset> Assets);