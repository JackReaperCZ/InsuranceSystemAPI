using InsuranceSystemAPI.Models;

namespace InsuranceSystemAPI.Services
{
    /// <summary>
    /// Služba pro GDPR compliance a správu osobních údajů
    /// </summary>
    public interface IGdprService
    {
        /// <summary>
        /// Exportuje všechna osobní data pojištěnce (právo na přenositelnost dat)
        /// </summary>
        /// <param name="pojistenecId">ID pojištěnce</param>
        /// <param name="requestedByUserId">ID uživatele, který požaduje export</param>
        /// <returns>Strukturovaná data ve formátu JSON</returns>
        Task<GdprDataExport> ExportPersonalDataAsync(int pojistenecId, int requestedByUserId);

        /// <summary>
        /// Anonymizuje osobní data pojištěnce (právo na výmaz)
        /// </summary>
        /// <param name="pojistenecId">ID pojištěnce</param>
        /// <param name="reason">Důvod anonymizace</param>
        /// <param name="anonymizedByUserId">ID uživatele, který provádí anonymizaci</param>
        /// <returns>True pokud byla anonymizace úspěšná</returns>
        Task<bool> AnonymizePersonalDataAsync(int pojistenecId, string reason, int anonymizedByUserId);

        /// <summary>
        /// Ověří, zda lze pojištěnce anonymizovat (nemá aktivní smlouvy)
        /// </summary>
        /// <param name="pojistenecId">ID pojištěnce</param>
        /// <returns>True pokud lze anonymizovat</returns>
        Task<bool> CanAnonymizeAsync(int pojistenecId);

        /// <summary>
        /// Zaloguje přístup k osobním datům pro audit
        /// </summary>
        /// <param name="pojistenecId">ID pojištěnce</param>
        /// <param name="userId">ID uživatele</param>
        /// <param name="action">Typ akce</param>
        /// <param name="details">Detaily akce</param>
        Task LogDataAccessAsync(int pojistenecId, int userId, GdprAction action, string details = "");

        /// <summary>
        /// Získá audit log pro pojištěnce
        /// </summary>
        /// <param name="pojistenecId">ID pojištěnce</param>
        /// <returns>Seznam audit záznamů</returns>
        Task<List<GdprAuditLog>> GetAuditLogAsync(int pojistenecId);

        /// <summary>
        /// Ověří souhlas se zpracováním osobních údajů
        /// </summary>
        /// <param name="pojistenecId">ID pojištěnce</param>
        /// <param name="category">Kategorie údajů</param>
        /// <returns>True pokud je souhlas platný</returns>
        Task<bool> HasValidConsentAsync(int pojistenecId, PersonalDataCategory category);

        /// <summary>
        /// Zaznamenává souhlas se zpracováním osobních údajů
        /// </summary>
        /// <param name="pojistenecId">ID pojištěnce</param>
        /// <param name="category">Kategorie údajů</param>
        /// <param name="purpose">Účel zpracování</param>
        /// <param name="consentGivenBy">Kdo udělil souhlas</param>
        Task RecordConsentAsync(int pojistenecId, PersonalDataCategory category, string purpose, int consentGivenBy);

        /// <summary>
        /// Odvolá souhlas se zpracováním osobních údajů
        /// </summary>
        /// <param name="pojistenecId">ID pojištěnce</param>
        /// <param name="category">Kategorie údajů</param>
        /// <param name="revokedBy">Kdo odvolal souhlas</param>
        Task RevokeConsentAsync(int pojistenecId, PersonalDataCategory category, int revokedBy);
    }

    /// <summary>
    /// Typ GDPR akce pro audit
    /// </summary>
    public enum GdprAction
    {
        /// <summary>
        /// Zobrazení osobních údajů
        /// </summary>
        View,

        /// <summary>
        /// Úprava osobních údajů
        /// </summary>
        Edit,

        /// <summary>
        /// Export osobních údajů
        /// </summary>
        Export,

        /// <summary>
        /// Export dat (alternativní název)
        /// </summary>
        DataExport,

        /// <summary>
        /// Anonymizace osobních údajů
        /// </summary>
        Anonymize,

        /// <summary>
        /// Anonymizace dat
        /// </summary>
        DataAnonymization,

        /// <summary>
        /// Udělení souhlasu
        /// </summary>
        ConsentGiven,

        /// <summary>
        /// Udělení souhlasu (alternativní název)
        /// </summary>
        ConsentGranted,

        /// <summary>
        /// Odvolání souhlasu
        /// </summary>
        ConsentRevoked,

        /// <summary>
        /// Vytvoření záznamu
        /// </summary>
        Create,

        /// <summary>
        /// Smazání záznamu
        /// </summary>
        Delete
    }

    /// <summary>
    /// Export osobních dat pro GDPR
    /// </summary>
    public class GdprDataExport
    {
        public int PojistenecId { get; set; }
        public DateTime ExportDate { get; set; }
        public PersonalData PersonalData { get; set; } = new();
        public List<ContractData> Contracts { get; set; } = new();
        public List<EventData> Events { get; set; } = new();
        public List<FileData> Files { get; set; } = new();
        public List<ConsentRecord> Consents { get; set; } = new();
    }

    /// <summary>
    /// Osobní data pojištěnce
    /// </summary>
    public class PersonalData
    {
        public string Jmeno { get; set; } = "";
        public string Prijmeni { get; set; } = "";
        public DateTime DatumNarozeni { get; set; }
        public string? Telefon { get; set; }
        public string? Email { get; set; }
        public string? Adresa { get; set; }
        public string? RodneCislo { get; set; }
        public string? CisloOP { get; set; }
        public DateTime DatumVytvoreni { get; set; }
        
        /// <summary>
        /// Slovník obsahující strukturovaná osobní data s metadaty
        /// </summary>
        public Dictionary<string, Dictionary<string, object>> Data { get; set; } = new();
    }

    /// <summary>
    /// Data pojistné smlouvy pro export
    /// </summary>
    public class ContractData
    {
        public int Id { get; set; }
        public string CisloSmlouvy { get; set; } = "";
        public string TypPojisteni { get; set; } = "";
        public decimal PojistnaCastka { get; set; }
        public decimal? LimitPojisteni { get; set; }
        public string Status { get; set; } = "";
        public DateTime PlatnostOd { get; set; }
        public DateTime PlatnostDo { get; set; }
        public DateTime DatumUzavreni { get; set; }
        public DateTime DatumPlatnostiOd { get; set; }
        public DateTime DatumPlatnostiDo { get; set; }
        public bool JeZaplacena { get; set; }
        public decimal RocniPojistne { get; set; }
        public DateTime DatumVytvoreni { get; set; }
        public List<FileData> Files { get; set; } = new();
        public List<EventData> Events { get; set; } = new();
    }

    /// <summary>
    /// Data pojistné události pro export
    /// </summary>
    public class EventData
    {
        public int Id { get; set; }
        public string CisloUdalosti { get; set; } = "";
        public string CisloPojistovny { get; set; } = "";
        public DateTime DatumCasUdalosti { get; set; }
        public DateTime DatumUdalosti { get; set; }
        public string PopisSkody { get; set; } = "";
        public string Popis { get; set; } = "";
        public string MistoUdalosti { get; set; } = "";
        public decimal? OdhadSkody { get; set; }
        public decimal? VyseSkody { get; set; }
        public decimal? CastkaPlneni { get; set; }
        public decimal? VyseNahrady { get; set; }
        public string Status { get; set; } = "";
        public DateTime DatumNahlaseni { get; set; }
        public DateTime? DatumVyrizeni { get; set; }
        public List<FileData> Files { get; set; } = new();
    }

    /// <summary>
    /// Data souboru pro export
    /// </summary>
    public class FileData
    {
        public int Id { get; set; }
        public string NazevSouboru { get; set; } = "";
        public string TypSouboru { get; set; } = "";
        public long VelikostSouboru { get; set; }
        public string? Popis { get; set; }
        public DateTime DatumNahrani { get; set; }
    }

    /// <summary>
    /// Záznam o souhlasu
    /// </summary>
    public class ConsentRecord
    {
        public PersonalDataCategory Category { get; set; }
        public string Purpose { get; set; } = "";
        public DateTime ConsentDate { get; set; }
        public DateTime? RevokedDate { get; set; }
        public bool IsActive { get; set; }
    }
}