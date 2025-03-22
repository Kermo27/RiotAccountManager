namespace RiotAccountManager.MAUI.Data;

public class Constants
{
    public static Dictionary<string, string> RegionMappings = new()
    {
        ["EUW"] = "euw1",
        ["NA"] = "na1",
        ["EUNE"] = "eun1",
        ["BR"] = "br1",
        ["TR"] = "tr1",
        ["RU"] = "ru",
        ["OCE"] = "oc1",
        ["LAN"] = "la1",
        ["LAS"] = "la2",
        ["JP"] = "jp1",
        ["KR"] = "kr"
    };

    public static List<string> Regions => RegionMappings.Keys.ToList();

    public static string GetRegionCode(string regionName)
    {
        if (RegionMappings.TryGetValue(regionName, out var code))
        {
            return code;
        }
        throw new ArgumentException($"Unknown region: {regionName}");
    }
}