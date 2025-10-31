using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceSystemAPI.Models
{
    [Table("ContractFiles")]
    public class ContractFile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string FileType { get; set; } = string.Empty; // .pdf, .docx, atd.

        public long FileSize { get; set; } // v bytech

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Foreign key - cizí klíč
        [Required]
        public int InsuranceContractId { get; set; }

        // Navigation property - navigační vlastnost
        [ForeignKey("InsuranceContractId")]
        public virtual InsuranceContract InsuranceContract { get; set; } = null!;
    }
}