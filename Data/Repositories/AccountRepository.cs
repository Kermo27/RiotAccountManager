using Newtonsoft.Json;
using RiotAccountManager.MAUI.Data.Models;
using RiotAccountManager.MAUI.Services.EncryptionService;

namespace RiotAccountManager.MAUI.Data.Repositories;

public class AccountRepository
{
    private readonly string _filePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "LeagueAccountManager",
        "accounts.json"
    );

    private readonly IEncryptionService _encryption;

    public AccountRepository(IEncryptionService encryption)
    {
        _encryption = encryption;
        EnsureDirectoryExists();
    }

    private void EnsureDirectoryExists()
    {
        var dir = Path.GetDirectoryName(_filePath);
        Directory.CreateDirectory(dir);
    }

    public List<Account> GetAll()
    {
        if (!File.Exists(_filePath))
            return new List<Account>();

        var json = File.ReadAllText(_filePath);

        return JsonConvert.DeserializeObject<List<Account>>(json);
    }

    public void SaveAll(List<Account> accounts)
    {
        try
        {
            var validAccounts = accounts
                .Where(a =>
                    !string.IsNullOrWhiteSpace(a.Username) && a.EncryptedPassword?.Length > 0
                )
                .ToList();

            var json = JsonConvert.SerializeObject(validAccounts);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Błąd zapisu danych", ex);
        }
    }
}
