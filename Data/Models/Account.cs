using Newtonsoft.Json;
using RiotAccountManager.MAUI.Services.EncryptionService;

namespace RiotAccountManager.MAUI.Data.Models;

public class Account
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Username { get; set; } = string.Empty;
    public byte[] EncryptedPassword { get; set; }

    [JsonIgnore]
    public string Password { get; set; }
    public string? Nickname { get; set; }
    public string Region { get; set; }

    public string GetDecryptedPassword(IEncryptionService encryptionService)
    {
        if (EncryptedPassword == null || EncryptedPassword.Length == 0)
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

    public bool IsExistingAccount()
    {
        return !string.IsNullOrEmpty(this.Id);
    }
}
