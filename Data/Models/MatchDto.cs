namespace RiotAccountManager.MAUI.Data.Models;

public class MatchDto
{
    public string MatchId { get; set; } = string.Empty;
    public string ChampionName { get; set; } = string.Empty;
    public int Kills { get; set; }
    public int Deaths { get; set; }
    public int Assists { get; set; }
    public bool Win { get; set; }
    public DateTimeOffset GameCreation { get; set; }
    public string QueueType { get; set; } = string.Empty;
    public string Lane { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int TotalDamageDealt { get; set; }
    public int VisionScore { get; set; }
    public TimeSpan GameDuration { get; set; }
}
