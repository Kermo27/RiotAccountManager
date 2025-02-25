namespace RiotAccountManager.MAUI.Services.RiotClientService;

public class RiotClientLockfileService : IRiotClientLockfileService
{
    private const int maxAttempts = 30;
    private const int delayMs = 1000;

    public async Task WaitForClientReady()
    {
        var lockfilePath = GetLockfilePath();

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            try
            {
                if (File.Exists(lockfilePath))
                {
                    using var fileStream = new FileStream(
                        lockfilePath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.ReadWrite
                    );

                    using var reader = new StreamReader(fileStream);
                    var content = await reader.ReadToEndAsync();

                    if (content.Split(':').Length >= 4)
                    {
                        return;
                    }
                }
            }
            catch (IOException)
            {
                throw new IOException("Błąd odczytu pliku lockfile");
            }

            await Task.Delay(delayMs);
        }

        throw new TimeoutException("Riot Client nie odpowiada");
    }

    private string? GetLockfilePath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Riot Games",
            "Riot Client",
            "Config",
            "lockfile"
        );
    }
}
