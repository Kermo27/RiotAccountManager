using RiotAccountManager.MAUI.Data.Models;

namespace RiotAccountManager.MAUI.Services.RiotApiService;

public interface IRiotApiService
{
    Task<LeagueStats> GetPlayerStats(string riotId, string region);
    Task<LeagueStats> GetCachedPlayerStats(string riotId, string region);
}