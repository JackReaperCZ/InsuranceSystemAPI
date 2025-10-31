using System.ComponentModel.DataAnnotations;
using InsuranceSystemAPI.Models;

namespace InsuranceSystemAPI.DTOs
{
    public class InsuranceContractDto
    {
        public int Id { get; set; }
        public string ContractNumber { get; set; } = string.Empty;
        public InsuranceType InsuranceType { get; set; }
        public string InsuranceTypeText { get; set; } = string.Empty;
        public decimal InsuredAmount { get; set; }
        public decimal InsuranceLimit { get; set; }
        public ContractStatus Status { get; set; }
        public string StatusText { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public decimal? AnnualPremium { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public int InsuredPersonId { get; set; }
        public string InsuredPersonName { get; set; } = string.Empty;
        public int? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public bool IsValid { get; set; }
        public int DaysToExpiry { get; set; }
        public int ClaimCount { get; set; }
    }

    public class CreateInsuranceContractDto
    {
        [Required]
        [StringLength(50)]
        public string ContractNumber { get; set; } = string.Empty;

        [Required]
        public InsuranceType InsuranceType { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal InsuredAmount { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal InsuranceLimit { get; set; }

        [Required]
        public DateTime ValidFrom { get; set; }

        [Required]
        public DateTime ValidTo { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? AnnualPremium { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [Required]
        public int InsuredPersonId { get; set; }

        public int? ManagerId { get; set; }

        public bool IsPaid { get; set; } = false;
    }

    public class UpdateInsuranceContractDto
    {
        [Required]
        public InsuranceType InsuranceType { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal InsuredAmount { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal InsuranceLimit { get; set; }

        [Required]
        public ContractStatus Status { get; set; }

        public bool IsPaid { get; set; }

        [Required]
        public DateTime ValidFrom { get; set; }

        [Required]
        public DateTime ValidTo { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? AnnualPremium { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public int? ManagerId { get; set; }
    }

    public class InsuranceContractSearchDto
    {
        public string? ContractNumber { get; set; }
        public InsuranceType? InsuranceType { get; set; }
        public ContractStatus? Status { get; set; }
        public int? InsuredPersonId { get; set; }
        public int? ManagerId { get; set; }
        public bool? IsPaid { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidFromFrom { get; set; }
        public DateTime? ValidFromTo { get; set; }
        public DateTime? ValidTo { get; set; }
        public string? InsuredPersonName { get; set; }
        public decimal? InsuredAmountFrom { get; set; }
        public decimal? InsuredAmountTo { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}