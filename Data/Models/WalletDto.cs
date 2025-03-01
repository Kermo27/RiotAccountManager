using System.Text.Json.Serialization;

namespace RiotAccountManager.MAUI.Data.Models;

public class WalletDto
{
    [JsonPropertyName("RP")]
    public int Rp { get; set; }

    [JsonPropertyName("lol_blue_essence")]
    public int LolBlueEssence { get; set; }
}
