namespace RiotAccountManager.MAUI.Helpers;

public static class RegionMapper
{
    private static readonly Dictionary<string, string> RegionMap = new(
        StringComparer.OrdinalIgnoreCase
    )
    {
        ["BR1"] = "BR",
        ["EUN1"] = "EUN",
        ["EUW1"] = "EUW",
        ["JP1"] = "JP",
        ["KR"] = "KR",
        ["LA1"] = "LA1",
        ["LA2"] = "LA2",
        ["NA1"] = "NA",
        ["OC1"] = "OC",
        ["RU"] = "RU",
        ["TR1"] = "TR",
    };

    private static readonly Dictionary<string, string> BigRegionMap = new()
    {
        ["BR"] = "AMERICAS",
        ["EUN"] = "EUROPE",
        ["EUW"] = "EUROPE",
        ["JP"] = "ASIA",
        ["KR"] = "ASIA",
        ["LA1"] = "AMERICAS",
        ["LA2"] = "AMERICAS",
        ["NA"] = "AMERICAS",
        ["OC"] = "SEA",
        ["RU"] = "EUROPE",
        ["TR"] = "EUROPE",
    };

    public static string MapToLibraryRegion(string region)
    {
        if (RegionMap.TryGetValue(region, out var mappedRegion))
            return mappedRegion;

        throw new ArgumentException($"Unknown region: {region}");
    }

    public static string GetBigRegion(string region)
    {
        var libraryRegion = MapToLibraryRegion(region);

        if (BigRegionMap.TryGetValue(libraryRegion, out var bigRegion))
            return bigRegion;

        throw new ArgumentException($"Unknown big region: {libraryRegion}");
    }
}
