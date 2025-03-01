namespace RiotAccountManager.MAUI.Data.Models;

public class RankedEntryDto
{
    public string Division { get; set; }
    public string HighestDivision { get; set; }
    public string HighestTier { get; set; }
    public bool IsProvisional { get; set; }
    public int LeaguePoints { get; set; }
    public int Losses { get; set; }
    public string MiniSeriesProgress { get; set; }
    public string PreviousSeasonEndDivision { get; set; }
    public string PreviousSeasonEndTier { get; set; }
    public string PreviousSeasonHighestDivision { get; set; }
    public string PreviousSeasonHighestTier { get; set; }
    public int ProvisionalGameThreshold { get; set; }
    public int ProvisionalGamesRemaining { get; set; }
    public string QueueType { get; set; }
    public int RatedRating { get; set; }
    public string RatedTier { get; set; }
    public string Tier { get; set; }
    public object Warnings { get; set; }
    public int Wins { get; set; }
    public string WinRatio => $"{Math.Round((float)Wins / (Wins + Losses) * 100)}%";
}
