using Newtonsoft.Json;
using RiotAccountManager.MAUI.Data.Models;
using RiotAccountManager.MAUI.Services.EncryptionService;

namespace RiotAccountManager.MAUI.Data.Repositories;

public class AccountRepository
{
    private readonly string _filePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "RiotAccountManager",
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
        if (dir != null)
        {
            Directory.CreateDirectory(dir);
        }
    }

    public List<Account> GetAll()
    {
        if (!File.Exists(_filePath))
            return new List<Account>();

        try
        {
            var json = File.ReadAllText(_filePath);

            if (string.IsNullOrWhiteSpace(json))
                return new List<Account>();

            var accounts = JsonConvert.DeserializeObject<List<Account>>(json);

            if (accounts == null)
                return new List<Account>();

            return accounts;
        }
        catch (Exception ex) when (ex is JsonReaderException || ex is JsonSerializationException)
        {
            Console.WriteLine($"Error loading accounts: {ex.Message}");
            return new List<Account>();
        }
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
