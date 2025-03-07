namespace RiotAccountManager.MAUI.Data.Models;

public class MatchHistoryItem
{
    public bool IsWin { get; set; }
    public string Champion { get; set; }
    public int ChampionLevel { get; set; }
    public int Kills { get; set; }
    public int Deaths { get; set; }
    public int Assists { get; set; }
    public int CS { get; set; }
    public int VisionScore { get; set; }
    public int[] Items { get; set; }
    public string Duration { get; set; }
    public string GameType { get; set; }
}
