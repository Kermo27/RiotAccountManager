using Microsoft.Extensions.Logging;

namespace RiotAccountManager.MAUI.Services.RiotClientService;

public class RiotClientLockfileService : IRiotClientLockfileService
{
    private const int MaxAttempts = 30;
    private const int DelayMs = 1000;
    private readonly ILogger<RiotClientLockfileService> _logger;

    public RiotClientLockfileService(ILogger<RiotClientLockfileService> logger)
    {
        _logger = logger;
    }

    public async Task WaitForClientReady()
    {
        var lockfilePath = GetLockfilePath();
        _logger.LogInformation("Waiting for Riot Client readiness. Lockfile path: {LockfilePath}", lockfilePath);

        for (int attempt = 1; attempt <= MaxAttempts; attempt++)
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

                    _logger.LogDebug("Attempt {Attempt}: Read lockfile content: {Content}", attempt, content);

                    if (content.Split(':').Length >= 4)
                    {
                        _logger.LogInformation("Lockfile format is valid. Riot Client is ready.");
                        
                        return;
                    }
                }
                else
                {
                    _logger.LogDebug("Attempt {Attempt}: Lockfile does not exist.", attempt);
                }
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Attempt {Attempt}: Error reading lockfile at path: {LockfilePath}", attempt, lockfilePath);
                throw new IOException("Error reading lockfile", ex);
            }

            await Task.Delay(DelayMs);
        }

        _logger.LogError("Exceeded maximum attempts ({MaxAttempts}). Riot Client is not responding.", MaxAttempts);
        throw new TimeoutException("Riot Client is not responding");
    }

    private string? GetLockfilePath()
    {
        var path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Riot Games",
            "Riot Client",
            "Config",
            "lockfile"
        );
        _logger.LogDebug("Calculated lockfile path: {Path}", path);
        
        return path;
    }
}
