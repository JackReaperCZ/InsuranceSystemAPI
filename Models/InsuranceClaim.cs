using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceSystemAPI.Models
{
    [Table("InsuranceClaims")]
    public class InsuranceClaim
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ClaimNumber { get; set; } = string.Empty;

        [Required]
        public DateTime IncidentDateTime { get; set; }

        [Required]
        [StringLength(1000)]
        public string DamageDescription { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string IncidentLocation { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Witnesses { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? EstimatedDamage { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MonetaryReserve { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PaymentAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DamageAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CompensationAmount { get; set; }

        [StringLength(100)]
        public string? InsuranceCompanyNumber { get; set; }

        [Required]
        public ClaimStatus Status { get; set; } = ClaimStatus.Open;

        [StringLength(2000)]
        public string? AdjusterNotes { get; set; }

        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ResolvedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Foreign keys - cizí klíče
        [Required]
        public int InsuranceContractId { get; set; }

        public int? AdjusterId { get; set; } // Likvidátor

        public int? ReporterId { get; set; } // Kdo událost nahlásil

        // Navigation properties - navigační vlastnosti
        [ForeignKey("InsuranceContractId")]
        public virtual InsuranceContract InsuranceContract { get; set; } = null!;

        [ForeignKey("AdjusterId")]
        public virtual User? Adjuster { get; set; }

        [ForeignKey("ReporterId")]
        public virtual User? Reporter { get; set; }

        public virtual ICollection<ClaimFile> AttachedFiles { get; set; } = new List<ClaimFile>();

        // Computed properties - vypočítané vlastnosti
        [NotMapped]
        public int DaysSinceReported => (DateTime.Now - ReportedAt).Days;

        [NotMapped]
        public bool IsResolved => Status == ClaimStatus.Resolved || Status == ClaimStatus.Rejected;
    }
}