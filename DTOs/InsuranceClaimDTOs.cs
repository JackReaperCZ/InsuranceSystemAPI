using System.ComponentModel.DataAnnotations;
using InsuranceSystemAPI.Models;

namespace InsuranceSystemAPI.DTOs
{
    public class InsuranceClaimDto
    {
        public int Id { get; set; }
        public DateTime IncidentDateTime { get; set; }
        public string DamageDescription { get; set; } = string.Empty;
        public string IncidentLocation { get; set; } = string.Empty;
        public string? Witnesses { get; set; }
        public decimal? EstimatedDamage { get; set; }
        public decimal? MonetaryReserve { get; set; }
        public decimal? PaymentAmount { get; set; }
        public string? InsuranceCompanyNumber { get; set; }
        public ClaimStatus ClaimStatus { get; set; }
        public string StatusText { get; set; } = string.Empty;
        public string? AdjusterNotes { get; set; }
        public DateTime ReportedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public int InsuranceContractId { get; set; }
        public string ContractNumber { get; set; } = string.Empty;
        public string InsuranceContractNumber { get; set; } = string.Empty;
        public string InsuredPersonName { get; set; } = string.Empty;
        public int? AdjusterId { get; set; }
        public string? AdjusterName { get; set; }
        public int? ReporterId { get; set; }
        public string? ReporterName { get; set; }
        public int DaysSinceReported { get; set; }
        public bool IsResolved { get; set; }
        public int FileCount { get; set; }
    }

    public class CreateInsuranceClaimDto
    {
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

        [Range(0, double.MaxValue)]
        public decimal? EstimatedDamage { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MonetaryReserve { get; set; }

        [StringLength(100)]
        public string? InsuranceCompanyNumber { get; set; }

        [Required]
        public int InsuranceContractId { get; set; }

        public int? AdjusterId { get; set; }

        public int? ReporterId { get; set; }
    }

    public class UpdateInsuranceClaimDto
    {
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

        [Range(0, double.MaxValue)]
        public decimal? EstimatedDamage { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MonetaryReserve { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? PaymentAmount { get; set; }

        [StringLength(100)]
        public string? InsuranceCompanyNumber { get; set; }

        [Required]
        public ClaimStatus ClaimStatus { get; set; }

        [StringLength(2000)]
        public string? AdjusterNotes { get; set; }

        public int? AdjusterId { get; set; }
    }

    public class InsuranceClaimSearchDto
    {
        public ClaimStatus? ClaimStatus { get; set; }
        public int? InsuranceContractId { get; set; }
        public int? InsuredPersonId { get; set; }
        public int? AdjusterId { get; set; }
        public DateTime? IncidentDateFrom { get; set; }
        public DateTime? IncidentDateTo { get; set; }
        public DateTime? ReportedDateFrom { get; set; }
        public DateTime? ReportedDateTo { get; set; }
        public string? InsuranceCompanyNumber { get; set; }
        public string? IncidentLocation { get; set; }
        public string? InsuredPersonName { get; set; }
        public decimal? EstimatedDamageFrom { get; set; }
        public decimal? EstimatedDamageTo { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}