using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceSystemAPI.Models
{
    [Table("ClaimFiles")]
    public class ClaimFile
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
        public string FileType { get; set; } = string.Empty; // .pdf, .docx, .jpg, .png

        public long FileSize { get; set; } // v bytech

        [Required]
        public FileCategory FileCategory { get; set; } // Dokument nebo Fotografie

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Foreign key - cizí klíč
        [Required]
        public int InsuranceClaimId { get; set; }

        // Navigation property - navigační vlastnost
        [ForeignKey("InsuranceClaimId")]
        public virtual InsuranceClaim InsuranceClaim { get; set; } = null!;
    }
}