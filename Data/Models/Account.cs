using RiotAccountManager.MAUI.Services;

namespace RiotAccountManager.MAUI.Data.Models;

public class Account
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Username { get; set; } = string.Empty;
    public byte[] EncryptedPassword { get; set; }
    public string? Nickname { get; set; }

    public string GetDecryptedPassword(EncryptionService encryptionService)
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
}
