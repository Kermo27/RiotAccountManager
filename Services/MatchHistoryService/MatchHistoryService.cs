using Kunc.RiotGames.Api;
using RiotAccountManager.MAUI.Data.Models;
using RiotAccountManager.MAUI.Helpers;

namespace RiotAccountManager.MAUI.Services.MatchHistoryService;

public class MatchHistoryService : IMatchHistoryService
{
    private readonly IPreferences _preferences;

    public MatchHistoryService(IPreferences preferences)
    {
        _preferences = preferences;
    }

    public async Task<List<MatchDto>> GetMatchHistoryAsync(
        string region,
        string summonerName,
        int maxMatches = 20
    )
    {
        var apiKey = _preferences.Get("riot_api_key", string.Empty);
        var api = RiotGamesApi.Create(x => x.ApiKey = apiKey);

        var properRegionName = region + "1";
        var bigRegion = RegionMapper.GetBigRegion(properRegionName);

        var account = await api.RiotAccountV1.GetAccountByRiotIdAsync(bigRegion, summonerName);
        var summoner = await api.LolSummonerV4.GetSummonerByPuuidAsync(properRegionName, account!.Puuid);

        var matchIds = await api.LolMatchV5.GetMatchIdsAsync(bigRegion, account.Puuid);

        return await ProcessMatchesAsync(api, bigRegion, summoner.Puuid, matchIds);
    }

    public async Task<List<MatchDto>> GetRankedMatchesAsync(
        string region,
        string summonerName,
        string queueType
    )
    {
        var queueId = queueType switch
        {
            "SOLO" => 420,
            "FLEX" => 440,
            _ => throw new ArgumentException("Invalid queue type"),
        };

        var apiKey = _preferences.Get("riot_api_key", string.Empty);
        var api = RiotGamesApi.Create(x => x.ApiKey = apiKey);

        var properRegionName = region + "1";
        var bigRegion = RegionMapper.GetBigRegion(properRegionName);

        var account = await api.RiotAccountV1.GetAccountByRiotIdAsync(bigRegion, summonerName);
        var summoner = await api.LolSummonerV4.GetSummonerByPuuidAsync(properRegionName, account!.Puuid);

        var matchIds = await api.LolMatchV5.GetMatchIdsAsync(bigRegion, account.Puuid);

        return await ProcessMatchesAsync(api, bigRegion, summoner.Puuid, matchIds);
    }

    private async Task<List<MatchDto>> ProcessMatchesAsync(
        IRiotGamesApi api,
        string bigRegion,
        string summonerPuuid,
        string[] matchIds
    )
    {
        var matches = new List<MatchDto>();

        foreach (var matchId in matchIds)
        {
            var match = await api.LolMatchV5.GetMatchAsync(bigRegion, matchId);
            var participant = match.Info.Participants.First(p => p.Puuid == summonerPuuid);

            matches.Add(
                new MatchDto
                {
                    MatchId = matchId,
                    ChampionName = participant.ChampionName,
                    Kills = participant.Kills,
                    Deaths = participant.Deaths,
                    Assists = participant.Assists,
                    Win = participant.IsWinner,
                    GameCreation = match.Info.GameCreation,
                    QueueType = GetQueueType(match.Info.QueueId),
                    Lane = participant.Lane.ToString(),
                    Role = participant.Role,
                    TotalDamageDealt = participant.TotalDamageDealt,
                    VisionScore = participant.VisionScore,
                    GameDuration = match.Info.GameDuration,
                }
            );
        }

        return matches;
    }

    private static string GetQueueType(int queueId) =>
        queueId switch
        {
            420 => "Solo/Duo",
            440 => "Flex",
            _ => "Other",
        };
}
