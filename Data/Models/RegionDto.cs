using System.Text.Json.Serialization;

namespace RiotAccountManager.MAUI.Data.Models;

public class RegionDto
{
    [JsonPropertyName("region")]
    public string Region { get; set; }
}
