using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceSystemAPI.Models
{
    [Table("InsuranceContracts")]
    public class InsuranceContract
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ContractNumber { get; set; } = string.Empty;

        [Required]
        public InsuranceType InsuranceType { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal InsuredAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal InsuranceLimit { get; set; }

        [Required]
        public ContractStatus Status { get; set; } = ContractStatus.Active;

        public bool IsPaid { get; set; } = false;

        [Required]
        [DataType(DataType.Date)]
        public DateTime ValidFrom { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ValidTo { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? AnnualPremium { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Foreign keys - cizí klíče
        [Required]
        public int InsuredPersonId { get; set; }

        public int? ManagerId { get; set; } // Makléř

        // Navigation properties - navigační vlastnosti
        [ForeignKey("InsuredPersonId")]
        public virtual InsuredPerson InsuredPerson { get; set; } = null!;

        [ForeignKey("ManagerId")]
        public virtual User? Manager { get; set; }

        public virtual ICollection<InsuranceClaim> InsuranceClaims { get; set; } = new List<InsuranceClaim>();
        public virtual ICollection<ContractFile> AttachedFiles { get; set; } = new List<ContractFile>();

        // Computed properties - vypočítané vlastnosti
        [NotMapped]
        public bool IsValid => DateTime.Now >= ValidFrom && DateTime.Now <= ValidTo && Status == ContractStatus.Active;

        [NotMapped]
        public int DaysToExpiry => (ValidTo - DateTime.Now).Days;
    }
}