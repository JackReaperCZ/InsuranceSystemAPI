using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InsuranceSystemAPI.Services;

namespace InsuranceSystemAPI.Models
{
    /// <summary>
    /// Audit log pro GDPR compliance - zaznamenává přístupy k osobním datům
    /// </summary>
    [Table("gdpr_audit_logs")]
    public class GdprAuditLog
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// ID uživatele, který provedl akci
        /// </summary>
        [Required]
        [Column("uzivatel_id")]
        public int UzivatelId { get; set; }

        /// <summary>
        /// ID pojištěnce, jehož data byla ovlivněna
        /// </summary>
        [Required]
        [Column("pojistenec_id")]
        public int PojistenecId { get; set; }

        /// <summary>
        /// Typ provedené akce
        /// </summary>
        [Required]
        [Column("akce")]
        [MaxLength(50)]
        public GdprAction Akce { get; set; }

        /// <summary>
        /// Detailní popis akce
        /// </summary>
        [Column("detaily")]
        [MaxLength(500)]
        public string? Detaily { get; set; }

        /// <summary>
        /// IP adresa uživatele
        /// </summary>
        [Column("ip_adresa")]
        [MaxLength(45)] // IPv6 může být až 45 znaků
        public string? IpAdresa { get; set; }

        /// <summary>
        /// User Agent prohlížeče
        /// </summary>
        [Column("user_agent")]
        [MaxLength(500)]
        public string? UserAgent { get; set; }

        /// <summary>
        /// Datum a čas akce
        /// </summary>
        [Required]
        [Column("datum_cas")]
        public DateTime DatumCas { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Navigační vlastnost k uživateli
        /// </summary>
        [ForeignKey("UzivatelId")]
        public virtual User? Uzivatel { get; set; }

        /// <summary>
        /// Navigační vlastnost k pojištěnci
        /// </summary>
        [ForeignKey("PojistenecId")]
        public virtual InsuredPerson? Pojistenec { get; set; }
    }
}