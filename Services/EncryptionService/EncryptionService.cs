using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace RiotAccountManager.MAUI.Services.EncryptionService;

public class EncryptionService : IEncryptionService, IDisposable
{
    private readonly byte[] _entropy;
    private readonly ILogger<EncryptionService> _logger;
    private bool _disposed;

    public EncryptionService(ILogger<EncryptionService> logger)
    {
        _logger = logger;
        _logger.LogInformation("Initializing EncryptionService.");
        _entropy = GenerateEntropy();
    }

    public byte[] Encrypt(string plainText)
    {
        _logger.LogInformation("Starting encryption operation.");
        
        try
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedData = ProtectedData.Protect(plainBytes, _entropy, DataProtectionScope.CurrentUser);
            _logger.LogInformation("Encryption completed successfully.");
            
            return encryptedData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during encryption.");
            throw;
        }
    }

    public string Decrypt(byte[] encryptedData)
    {
        _logger.LogInformation("Starting decryption operation.");
        try
        {
            var decryptedBytes = ProtectedData.Unprotect(encryptedData, _entropy, DataProtectionScope.CurrentUser);
            var decryptedText = Encoding.UTF8.GetString(decryptedBytes);
            _logger.LogInformation("Decryption completed successfully.");
            
            return decryptedText;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during decryption.");
            throw;
        }
    }

    private byte[] GenerateEntropy()
    {
        _logger.LogInformation("Generating entropy for EncryptionService.");
        var machineGuid = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography", "MachineGuid", "")?.ToString();

        if (string.IsNullOrEmpty(machineGuid))
        {
            _logger.LogWarning("MachineGuid not found in registry; defaulting entropy value.");
        }

        return Encoding.UTF8.GetBytes(machineGuid + "LeagueSecretSalt");
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _logger.LogInformation("Disposing EncryptionService.");
            Array.Clear(_entropy, 0, _entropy.Length);
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
