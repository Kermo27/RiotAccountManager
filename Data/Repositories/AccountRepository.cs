using System.Text.Json;
using Microsoft.Extensions.Logging;
using RiotAccountManager.MAUI.Data.Models;

namespace RiotAccountManager.MAUI.Data.Repositories;

public class AccountRepository
{
    private readonly string _filePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "RiotAccountManager",
        "accounts.json");
    private readonly ILogger<AccountRepository> _logger;

    public AccountRepository(ILogger<AccountRepository> logger)
    {
        _logger = logger;
        EnsureDirectoryExists();
    }

    private void EnsureDirectoryExists()
    {
        var dir = Path.GetDirectoryName(_filePath);
        if (dir != null)
        {
            Directory.CreateDirectory(dir);
        }
    }

    public async Task<List<Account>> GetAllAsync()
    {
        if (!File.Exists(_filePath))
        {
            _logger.LogInformation("Accounts file not found. Returning an empty list.");
            
            return new List<Account>();
        }

        try
        {
            await using var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
            var accounts = await JsonSerializer.DeserializeAsync<List<Account>>(stream);
            
            return accounts ?? new List<Account>();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing accounts from JSON.");
            
            return new List<Account>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error reading accounts file.");
            
            return new List<Account>();
        }
    }

    public async Task SaveAllAsync(List<Account> accounts)
    {
        try
        {
            var validAccounts = accounts
                .Where(a => !string.IsNullOrWhiteSpace(a.Username) && a.EncryptedPassword?.Length > 0)
                .ToList();

            await using var stream = new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
            await JsonSerializer.SerializeAsync(stream, validAccounts, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving accounts to file.");
            throw new ApplicationException("Error saving account data.", ex);
        }
    }
}
