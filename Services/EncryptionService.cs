using System.Security.Cryptography;
using System.Text;

namespace InsuranceSystemAPI.Services
{
    /// <summary>
    /// Implementace služby pro šifrování citlivých dat
    /// </summary>
    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public EncryptionService(IConfiguration configuration)
        {
            var encryptionKey = configuration["Encryption:Key"] ?? throw new InvalidOperationException("Encryption key not configured");
            var encryptionIV = configuration["Encryption:IV"] ?? throw new InvalidOperationException("Encryption IV not configured");

            _key = Convert.FromBase64String(encryptionKey);
            _iv = Convert.FromBase64String(encryptionIV);

            // Ověření délky klíče a IV
            if (_key.Length != 32) // 256 bitů
                throw new InvalidOperationException("Encryption key must be 256 bits (32 bytes)");
            if (_iv.Length != 16) // 128 bitů
                throw new InvalidOperationException("Encryption IV must be 128 bits (16 bytes)");
        }

        /// <summary>
        /// Zašifruje text pomocí AES-256-CBC
        /// </summary>
        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            try
            {
                using var aes = Aes.Create();
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var encryptor = aes.CreateEncryptor();
                using var msEncrypt = new MemoryStream();
                using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                using var swEncrypt = new StreamWriter(csEncrypt);

                swEncrypt.Write(plainText);
                swEncrypt.Close();

                return Convert.ToBase64String(msEncrypt.ToArray());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Chyba při šifrování dat", ex);
            }
        }

        /// <summary>
        /// Dešifruje text pomocí AES-256-CBC
        /// </summary>
        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            try
            {
                var cipherBytes = Convert.FromBase64String(cipherText);

                using var aes = Aes.Create();
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var decryptor = aes.CreateDecryptor();
                using var msDecrypt = new MemoryStream(cipherBytes);
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt);

                return srDecrypt.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Chyba při dešifrování dat", ex);
            }
        }

        /// <summary>
        /// Vytvoří SHA256 hash pro vyhledávání
        /// </summary>
        public string CreateHash(string data)
        {
            if (string.IsNullOrEmpty(data))
                return string.Empty;

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data.ToLowerInvariant()));
            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Ověří hash
        /// </summary>
        public bool VerifyHash(string data, string hash)
        {
            if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(hash))
                return false;

            var computedHash = CreateHash(data);
            return computedHash.Equals(hash, StringComparison.Ordinal);
        }
    }

    /// <summary>
    /// Pomocná třída pro generování šifrovacích klíčů
    /// </summary>
    public static class EncryptionKeyGenerator
    {
        /// <summary>
        /// Vygeneruje nový 256-bitový klíč pro AES
        /// </summary>
        public static string GenerateKey()
        {
            using var aes = Aes.Create();
            aes.GenerateKey();
            return Convert.ToBase64String(aes.Key);
        }

        /// <summary>
        /// Vygeneruje nový 128-bitový IV pro AES
        /// </summary>
        public static string GenerateIV()
        {
            using var aes = Aes.Create();
            aes.GenerateIV();
            return Convert.ToBase64String(aes.IV);
        }

        /// <summary>
        /// Vygeneruje klíč a IV pro konfiguraci
        /// </summary>
        public static (string Key, string IV) GenerateKeyAndIV()
        {
            return (GenerateKey(), GenerateIV());
        }
    }
}