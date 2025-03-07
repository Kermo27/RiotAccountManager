using RiotAccountManager.MAUI.Data.Models;

namespace RiotAccountManager.MAUI.Services.MatchHistoryService;

public interface IMatchHistoryService
{
    Task<List<MatchDto>> GetMatchHistoryAsync(
        string region,
        string summonerName,
        int maxMatches = 20
    );
    Task<List<MatchDto>> GetRankedMatchesAsync(
        string region,
        string summonerName,
        string queueType
    );
}
