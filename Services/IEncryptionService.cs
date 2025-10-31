using System.ComponentModel.DataAnnotations;

namespace InsuranceSystemAPI.Services
{
    /// <summary>
    /// Služba pro šifrování a dešifrování citlivých dat
    /// </summary>
    public interface IEncryptionService
    {
        /// <summary>
        /// Zašifruje text pomocí AES algoritmu
        /// </summary>
        /// <param name="plainText">Text k zašifrování</param>
        /// <returns>Zašifrovaný text v Base64 formátu</returns>
        string Encrypt(string plainText);

        /// <summary>
        /// Dešifruje text pomocí AES algoritmu
        /// </summary>
        /// <param name="cipherText">Zašifrovaný text v Base64 formátu</param>
        /// <returns>Dešifrovaný text</returns>
        string Decrypt(string cipherText);

        /// <summary>
        /// Vytvoří hash z citlivých dat pro vyhledávání
        /// </summary>
        /// <param name="data">Data k zahashování</param>
        /// <returns>SHA256 hash</returns>
        string CreateHash(string data);

        /// <summary>
        /// Ověří, zda data odpovídají hashi
        /// </summary>
        /// <param name="data">Data k ověření</param>
        /// <param name="hash">Hash k porovnání</param>
        /// <returns>True pokud data odpovídají hashi</returns>
        bool VerifyHash(string data, string hash);
    }

    /// <summary>
    /// Atribut pro označení vlastností, které mají být šifrovány
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EncryptedAttribute : Attribute
    {
        /// <summary>
        /// Určuje, zda má být vytvořen hash pro vyhledávání
        /// </summary>
        public bool CreateSearchHash { get; set; } = false;
    }

    /// <summary>
    /// Atribut pro označení vlastností obsahujících osobní údaje (GDPR)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PersonalDataAttribute : Attribute
    {
        /// <summary>
        /// Kategorie osobních údajů
        /// </summary>
        public PersonalDataCategory Category { get; set; } = PersonalDataCategory.General;

        /// <summary>
        /// Určuje, zda jsou data povinná pro fungování systému
        /// </summary>
        public bool IsRequired { get; set; } = false;

        /// <summary>
        /// Popis účelu zpracování
        /// </summary>
        public string Purpose { get; set; } = "";
    }

    /// <summary>
    /// Kategorie osobních údajů podle GDPR
    /// </summary>
    public enum PersonalDataCategory
    {
        /// <summary>
        /// Obecné osobní údaje
        /// </summary>
        General,

        /// <summary>
        /// Identifikační údaje
        /// </summary>
        Identification,

        /// <summary>
        /// Kontaktní údaje
        /// </summary>
        Contact,

        /// <summary>
        /// Finanční údaje
        /// </summary>
        Financial,

        /// <summary>
        /// Zdravotní údaje
        /// </summary>
        Health,

        /// <summary>
        /// Citlivé osobní údaje
        /// </summary>
        Sensitive
    }
}