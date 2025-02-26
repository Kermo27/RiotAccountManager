namespace RiotAccountManager.MAUI.Data.Models;

public class CurrentRankedStatsDto
{
    public int CurrentSeasonSplitPoints { get; set; }
    public List<string> EarnedRegaliaRewardIds { get; set; }
    public string HighestCurrentSeasonReachedTierSR { get; set; }
    public string HighestPreviousSeasonEndDivision { get; set; }
    public string HighestPreviousSeasonEndTier { get; set; }
    public RankedEntryDto HighestRankedEntry { get; set; }
    public RankedEntryDto HighestRankedEntrySR { get; set; }
    public int PreviousSeasonSplitPoints { get; set; }
}
