using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;

namespace RiotAccountManager.MAUI.Services.EncryptionService;

public class EncryptionService : IEncryptionService, IDisposable
{
    private readonly byte[] _entropy;
    private bool _disposed;

    public EncryptionService()
    {
        _entropy = GenerateEntropy();
    }

    public byte[] Encrypt(string plainText)
    {
        var plainBytes = Encoding.UTF8.GetBytes(plainText);

        return ProtectedData.Protect(plainBytes, _entropy, DataProtectionScope.CurrentUser);
    }

    public string Decrypt(byte[] encryptedData)
    {
        var decryptedBytes = ProtectedData.Unprotect(
            encryptedData,
            _entropy,
            DataProtectionScope.CurrentUser
        );

        return Encoding.UTF8.GetString(decryptedBytes);
    }

    private byte[] GenerateEntropy()
    {
        var machineGuid = Registry
            .GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography", "MachineGuid", "")
            ?.ToString();
        return Encoding.UTF8.GetBytes(machineGuid + "LeagueSecretSalt");
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            Array.Clear(_entropy, 0, _entropy.Length);
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
