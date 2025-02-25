namespace RiotAccountManager.MAUI.Services.EncryptionService;

public interface IEncryptionService
{
    byte[] Encrypt(string plainText);
    string Decrypt(byte[] encryptedData);
}
