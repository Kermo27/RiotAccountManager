using Newtonsoft.Json;
using RiotAccountManager.MAUI.Services.EncryptionService;

namespace RiotAccountManager.MAUI.Data.Models;

public class Account
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Username { get; set; } = string.Empty;
    public byte[] EncryptedPassword { get; set; } = null!;

    [JsonIgnore]
    public string Password { get; set; } = string.Empty;
    public string? Nickname { get; set; }
    public string Region { get; set; } = string.Empty;

    public string GetDecryptedPassword(IEncryptionService encryptionService)
    {
        if (EncryptedPassword.Length == 0)
            return string.Empty;

        try
        {
            return encryptionService.Decrypt(EncryptedPassword);
        }
        catch
        {
            return "DECRYPTION_ERROR";
        }
    }

    public static bool IsExistingAccount(string id)
    {
        return !string.IsNullOrEmpty(id);
    }
}
