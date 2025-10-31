using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InsuranceSystemAPI.Services;

namespace InsuranceSystemAPI.Models
{
    /// <summary>
    /// Záznam o souhlasu se zpracováním osobních údajů podle GDPR
    /// </summary>
    [Table("gdpr_consents")]
    public class GdprConsent
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// ID pojištěnce, který udělil souhlas
        /// </summary>
        [Required]
        [Column("pojistenec_id")]
        public int PojistenecId { get; set; }

        /// <summary>
        /// Kategorie osobních údajů
        /// </summary>
        [Required]
        [Column("kategorie")]
        public PersonalDataCategory Kategorie { get; set; }

        /// <summary>
        /// Účel zpracování osobních údajů
        /// </summary>
        [Required]
        [Column("ucel")]
        [MaxLength(500)]
        public string Ucel { get; set; } = "";

        /// <summary>
        /// Datum a čas udělení souhlasu
        /// </summary>
        [Required]
        [Column("datum_udeleni")]
        public DateTime DatumUdeleni { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// ID uživatele, který zaznamenal souhlas
        /// </summary>
        [Required]
        [Column("udeljen_kym")]
        public int UdelenoKym { get; set; }

        /// <summary>
        /// Datum a čas odvolání souhlasu
        /// </summary>
        [Column("datum_odvolani")]
        public DateTime? DatumOdvolani { get; set; }

        /// <summary>
        /// ID uživatele, který zaznamenal odvolání
        /// </summary>
        [Column("odvolano_kym")]
        public int? OdvolanoKym { get; set; }

        /// <summary>
        /// Důvod odvolání souhlasu
        /// </summary>
        [Column("duvod_odvolani")]
        [MaxLength(500)]
        public string? DuvodOdvolani { get; set; }

        /// <summary>
        /// Zda je souhlas aktivní
        /// </summary>
        [Required]
        [Column("je_aktivni")]
        public bool JeAktivni { get; set; } = true;

        /// <summary>
        /// Verze podmínek souhlasu
        /// </summary>
        [Required]
        [Column("verze_podminek")]
        [MaxLength(20)]
        public string VerzePodminek { get; set; } = "1.0";

        /// <summary>
        /// IP adresa při udělení souhlasu
        /// </summary>
        [Column("ip_adresa")]
        [MaxLength(45)]
        public string? IpAdresa { get; set; }

        /// <summary>
        /// Navigační vlastnost k pojištěnci
        /// </summary>
        [ForeignKey("PojistenecId")]
        public virtual InsuredPerson? Pojistenec { get; set; }

        /// <summary>
        /// Navigační vlastnost k uživateli, který udělil souhlas
        /// </summary>
        [ForeignKey("UdelenoKym")]
        public virtual User? UdelilUzivatel { get; set; }

        /// <summary>
        /// Navigační vlastnost k uživateli, který odvolal souhlas
        /// </summary>
        [ForeignKey("OdvolanoKym")]
        public virtual User? OdvolalUzivatel { get; set; }
    }
}