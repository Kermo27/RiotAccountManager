namespace RiotAccountManager.MAUI.Data.Models;

public class LeagueStats
{
    public string SummonerName { get; set; }
    public string RankSolo { get; set; }
    public int LeaguePoints { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }

    public decimal WinRate =>
    (Wins + Losses) > 0
        ? Math.Round((decimal)Wins / (Wins + Losses) * 100, 2)
        : 0m;

    public List<ChampionMasteryInfo> TopChampions { get; set; } = new();
    public string? ErrorMessage { get; set; }
}