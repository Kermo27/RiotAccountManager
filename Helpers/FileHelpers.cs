namespace RiotAccountManager.MAUI.Helpers;

public static class FileHelpers
{
    public static bool AppPackageFileExists(string filename)
    {
        try
        {
            using var stream = FileSystem.OpenAppPackageFileAsync(filename).Result;
            return stream != null;
        }
        catch
        {
            return false;
        }
    }
}
