using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InsuranceSystemAPI.Services;

namespace InsuranceSystemAPI.Models
{
    [Table("InsuredPersons")]
    public class InsuredPerson
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Encrypted(CreateSearchHash = true)]
        [PersonalData(Category = PersonalDataCategory.General, IsRequired = true, Purpose = "Křestní jméno pojištěnce")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Encrypted(CreateSearchHash = true)]
        [PersonalData(Category = PersonalDataCategory.General, IsRequired = true, Purpose = "Příjmení pojištěnce")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [PersonalData(Category = PersonalDataCategory.General, IsRequired = true, Purpose = "Datum narození pro výpočet věku a rizika")]
        public DateTime DateOfBirth { get; set; }

        [StringLength(20)]
        [Encrypted]
        [PersonalData(Category = PersonalDataCategory.Contact, IsRequired = false, Purpose = "Telefonní číslo pro kontakt")]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(255)]
        [Encrypted(CreateSearchHash = true)]
        [PersonalData(Category = PersonalDataCategory.Contact, IsRequired = false, Purpose = "E-mailová adresa pro komunikaci")]
        public string? Email { get; set; }

        [StringLength(500)]
        [Encrypted]
        [PersonalData(Category = PersonalDataCategory.Contact, IsRequired = false, Purpose = "Adresa bydliště")]
        public string? Address { get; set; }

        [StringLength(20)]
        [Encrypted]
        [PersonalData(Category = PersonalDataCategory.Sensitive, IsRequired = false, Purpose = "Rodné číslo pro identifikaci")]
        public string? NationalId { get; set; }

        [StringLength(50)]
        [Encrypted]
        [PersonalData(Category = PersonalDataCategory.Identification, IsRequired = false, Purpose = "Číslo občanského průkazu")]
        public string? IdCardNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties - navigační vlastnosti
        public virtual ICollection<InsuranceContract> InsuranceContracts { get; set; } = new List<InsuranceContract>();
        public virtual ICollection<InsuranceClaim> InsuranceClaims { get; set; } = new List<InsuranceClaim>();

        // Computed property - vypočítané vlastnosti
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        [NotMapped]
        public int Age => DateTime.Now.Year - DateOfBirth.Year - 
                         (DateTime.Now.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
    }
}