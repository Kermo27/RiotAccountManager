using Kunc.RiotGames.Api;
using Kunc.RiotGames.Lol;
using Kunc.RiotGames.Lol.DataDragon;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using RiotAccountManager.MAUI.Data.Models;
using Constants = RiotAccountManager.MAUI.Data.Constants;

namespace RiotAccountManager.MAUI.Services.RiotApiService;

public class RiotApiService : IRiotApiService
{
    private readonly IRiotGamesApi _api;
    private readonly ILolDataDragon _dataDragon;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

    public RiotApiService(IConfiguration configuration, IMemoryCache cache)
    {
        string apiKey = configuration["RiotGames:ApiKey"];
        _api = RiotGamesApi.Create(x => x.ApiKey = apiKey);
        _dataDragon = LolDataDragon.Create();
        _cache = cache;
    }

    public async Task<LeagueStats> GetPlayerStats(string riotId, string region)
    {
        var stats = new LeagueStats();
    
    try
    {
        var smallRegion = Constants.GetRegionCode(region);
        
        var account = await _api.RiotAccountV1.GetAccountByRiotIdAsync(Regions.EUROPE, riotId);
        if (account == null)
        {
            stats.ErrorMessage = "Account not found";
            return stats;
        }
        
        var summoner = await _api.LolSummonerV4.GetSummonerByPuuidAsync(smallRegion, account.Puuid);
        if (summoner == null)
        {
            stats.ErrorMessage = "Summoner info unavailable";
            return stats;
        }
        
        var entries = await _api.LolLeagueV4.LeagueEntriesForSummonerAsync(smallRegion, summoner.Id);
        var soloQueue = entries?.FirstOrDefault(e => e.QueueType == QueueType.RankedSolo5x5);
        
        var masteries = await _api.LolChampionMasteryV4.GetAllChampionMasteryEntriesAsync(smallRegion, account.Puuid);
        var champions = await _dataDragon.GetChampionsBaseAsync("latest", "en_US");
        
        stats.SummonerName = riotId;
        stats.RankSolo = soloQueue?.ToRank().ToString() ?? "Unranked";
        stats.LeaguePoints = soloQueue?.LeaguePoints ?? 0;
        stats.Wins = soloQueue?.Wins ?? 0;
        stats.Losses = soloQueue?.Losses ?? 0;

        stats.TopChampions = masteries?
            .Take(3)
            .Select(m => 
            {
                var champion = champions?.FirstOrDefault(c => c.Value.Key == m.ChampionId).Value;
                return new ChampionMasteryInfo
                {
                    ChampionName = champion?.Name ?? "Unknown",
                    ChampionLevel = m.ChampionLevel,
                    MasteryPoints = m.ChampionPoints
                };
            })
            .ToList() ?? new List<ChampionMasteryInfo>();

        return stats;
    }
    catch (Exception ex)
    {
        stats.ErrorMessage = $"Unexpected error: {ex.Message}";
        return stats;
    }
    }
    
    public async Task<LeagueStats> GetCachedPlayerStats(string riotId, string region)
    {
        var cacheKey = $"{region}_{riotId}_stats";
    
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            var stats = await GetPlayerStats(riotId, region);
        
            // Cache errors for shorter time
            if (!string.IsNullOrEmpty(stats.ErrorMessage))
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            }
            else
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            }
        
            return stats;
        });
    }
}