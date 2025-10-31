using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InsuranceSystemAPI.Data;
using InsuranceSystemAPI.DTOs;
using InsuranceSystemAPI.Models;

namespace InsuranceSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InsuranceContractsController : ControllerBase
    {
        private readonly InsuranceDbContext _context;

        public InsuranceContractsController(InsuranceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Získání seznamu pojistných smluv s možností vyhledávání a stránkování
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<InsuranceContractDto>>> GetInsuranceContracts([FromQuery] InsuranceContractSearchDto search)
        {
            var query = _context.InsuranceContracts
                .Include(s => s.InsuredPerson)
                .Include(s => s.Manager)
                .Include(s => s.InsuranceClaims)
                .AsQueryable();

            // Filtrování
            if (!string.IsNullOrEmpty(search.ContractNumber))
                query = query.Where(s => s.ContractNumber.Contains(search.ContractNumber));

            if (search.InsuranceType.HasValue)
                query = query.Where(s => s.InsuranceType == search.InsuranceType.Value);

            if (search.Status.HasValue)
                query = query.Where(s => s.Status == search.Status.Value);

            if (search.IsPaid.HasValue)
                query = query.Where(s => s.IsPaid == search.IsPaid.Value);

            if (search.ValidFrom.HasValue)
                query = query.Where(s => s.ValidFrom >= search.ValidFrom.Value);

            if (search.ValidTo.HasValue)
                query = query.Where(s => s.ValidTo <= search.ValidTo.Value);

            if (search.InsuredPersonId.HasValue)
                query = query.Where(s => s.InsuredPersonId == search.InsuredPersonId.Value);

            if (search.ManagerId.HasValue)
                query = query.Where(s => s.ManagerId == search.ManagerId.Value);

            if (!string.IsNullOrEmpty(search.InsuredPersonName))
                query = query.Where(s => (s.InsuredPerson.FirstName + " " + s.InsuredPerson.LastName).Contains(search.InsuredPersonName));

            var totalCount = await query.CountAsync();

            var pageEntities = await query
                .OrderByDescending(s => s.CreatedAt)
                .Skip((search.Page - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToListAsync();

            var contracts = pageEntities
                .Select(s => new InsuranceContractDto
                {
                    Id = s.Id,
                    ContractNumber = s.ContractNumber,
                    InsuranceType = s.InsuranceType,
                    InsuranceTypeText = s.InsuranceType.ToString(),
                    InsuredAmount = s.InsuredAmount,
                    InsuranceLimit = s.InsuranceLimit,
                    Status = s.Status,
                    StatusText = s.Status.ToString(),
                    IsPaid = s.IsPaid,
                    ValidFrom = s.ValidFrom,
                    ValidTo = s.ValidTo,
                    AnnualPremium = s.AnnualPremium,
                    Notes = s.Notes,
                    CreatedAt = s.CreatedAt,
                    InsuredPersonId = s.InsuredPersonId,
                    InsuredPersonName = s.InsuredPerson != null ? ($"{s.InsuredPerson.FirstName} {s.InsuredPerson.LastName}") : string.Empty,
                    ManagerId = s.ManagerId,
                    ManagerName = s.Manager != null ? ($"{s.Manager.FirstName} {s.Manager.LastName}") : null,
                    IsValid = DateTime.Now >= s.ValidFrom && DateTime.Now <= s.ValidTo && s.Status == ContractStatus.Active,
                    DaysToExpiry = (s.ValidTo - DateTime.Now).Days,
                    ClaimCount = (s.InsuranceClaims != null ? s.InsuranceClaims.Count : 0)
                })
                .ToList();

            return Ok(new PagedResult<InsuranceContractDto>
            {
                Items = contracts,
                TotalCount = totalCount,
                Page = search.Page,
                PageSize = search.PageSize
            });
        }

        /// <summary>
        /// Získání detailu pojistné smlouvy podle ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<InsuranceContractDto>> GetInsuranceContract(int id)
        {
            var contract = await _context.InsuranceContracts
                .Include(s => s.InsuredPerson)
                .Include(s => s.Manager)
                .Include(s => s.InsuranceClaims)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (contract == null)
                return NotFound();

            var dto = new InsuranceContractDto
            {
                Id = contract.Id,
                ContractNumber = contract.ContractNumber,
                InsuranceType = contract.InsuranceType,
                InsuranceTypeText = contract.InsuranceType.ToString(),
                InsuredAmount = contract.InsuredAmount,
                InsuranceLimit = contract.InsuranceLimit,
                Status = contract.Status,
                StatusText = contract.Status.ToString(),
                IsPaid = contract.IsPaid,
                ValidFrom = contract.ValidFrom,
                ValidTo = contract.ValidTo,
                AnnualPremium = contract.AnnualPremium,
                Notes = contract.Notes,
                CreatedAt = contract.CreatedAt,
                InsuredPersonId = contract.InsuredPersonId,
                InsuredPersonName = $"{contract.InsuredPerson.FirstName} {contract.InsuredPerson.LastName}",
                ManagerId = contract.ManagerId,
                ManagerName = contract.Manager != null ? $"{contract.Manager.FirstName} {contract.Manager.LastName}" : null,
                IsValid = DateTime.Now >= contract.ValidFrom && DateTime.Now <= contract.ValidTo && contract.Status == ContractStatus.Active,
                DaysToExpiry = (contract.ValidTo - DateTime.Now).Days,
                ClaimCount = contract.InsuranceClaims.Count
            };

            return Ok(dto);
        }

        /// <summary>
        /// Vytvoření nové pojistné smlouvy
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Broker")]
        public async Task<ActionResult<InsuranceContractDto>> CreateInsuranceContract([FromBody] CreateInsuranceContractDto createDto)
        {
            // Kontrola existence pojištěnce
            var insuredPerson = await _context.InsuredPersons.FindAsync(createDto.InsuredPersonId);
            if (insuredPerson == null)
                return BadRequest("Pojištěnec neexistuje");

            // Kontrola existence správce
            if (createDto.ManagerId.HasValue)
            {
                var manager = await _context.Users.FindAsync(createDto.ManagerId.Value);
                if (manager == null)
                    return BadRequest("Správce neexistuje");
            }

            // Generování čísla smlouvy
            var contractNumber = await GenerateContractNumber();

            var contract = new InsuranceContract
            {
                ContractNumber = contractNumber,
                InsuranceType = createDto.InsuranceType,
                InsuredAmount = createDto.InsuredAmount,
                InsuranceLimit = createDto.InsuranceLimit,
                Status = ContractStatus.Active,
                IsPaid = createDto.IsPaid,
                ValidFrom = createDto.ValidFrom,
                ValidTo = createDto.ValidTo,
                AnnualPremium = createDto.AnnualPremium,
                Notes = createDto.Notes,
                CreatedAt = DateTime.UtcNow,
                InsuredPersonId = createDto.InsuredPersonId,
                ManagerId = createDto.ManagerId
            };

            _context.InsuranceContracts.Add(contract);
            await _context.SaveChangesAsync();

            // Načtení s navigačními vlastnostmi pro response
            await _context.Entry(contract)
                .Reference(s => s.InsuredPerson)
                .LoadAsync();

            if (contract.ManagerId.HasValue)
            {
                await _context.Entry(contract)
                    .Reference(s => s.Manager)
                    .LoadAsync();
            }

            var dto = new InsuranceContractDto
            {
                Id = contract.Id,
                ContractNumber = contract.ContractNumber,
                InsuranceType = contract.InsuranceType,
                InsuranceTypeText = contract.InsuranceType.ToString(),
                InsuredAmount = contract.InsuredAmount,
                InsuranceLimit = contract.InsuranceLimit,
                Status = contract.Status,
                StatusText = contract.Status.ToString(),
                IsPaid = contract.IsPaid,
                ValidFrom = contract.ValidFrom,
                ValidTo = contract.ValidTo,
                AnnualPremium = contract.AnnualPremium,
                Notes = contract.Notes,
                CreatedAt = contract.CreatedAt,
                InsuredPersonId = contract.InsuredPersonId,
                InsuredPersonName = $"{contract.InsuredPerson.FirstName} {contract.InsuredPerson.LastName}",
                ManagerId = contract.ManagerId,
                ManagerName = contract.Manager != null ? $"{contract.Manager.FirstName} {contract.Manager.LastName}" : null,
                IsValid = DateTime.Now >= contract.ValidFrom && DateTime.Now <= contract.ValidTo && contract.Status == ContractStatus.Active,
                DaysToExpiry = (contract.ValidTo - DateTime.Now).Days,
                ClaimCount = 0
            };

            return CreatedAtAction(nameof(GetInsuranceContract), new { id = contract.Id }, dto);
        }

        /// <summary>
        /// Aktualizace pojistné smlouvy
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Broker")]
        public async Task<IActionResult> UpdateInsuranceContract(int id, [FromBody] UpdateInsuranceContractDto updateDto)
        {
            var contract = await _context.InsuranceContracts.FindAsync(id);
            if (contract == null)
                return NotFound();

            // Kontrola existence správce
            if (updateDto.ManagerId.HasValue)
            {
                var manager = await _context.Users.FindAsync(updateDto.ManagerId.Value);
                if (manager == null)
                    return BadRequest("Správce neexistuje");
            }

            contract.InsuranceType = updateDto.InsuranceType;
            contract.InsuredAmount = updateDto.InsuredAmount;
            contract.InsuranceLimit = updateDto.InsuranceLimit;
            contract.Status = updateDto.Status;
            contract.IsPaid = updateDto.IsPaid;
            contract.ValidFrom = updateDto.ValidFrom;
            contract.ValidTo = updateDto.ValidTo;
            contract.AnnualPremium = updateDto.AnnualPremium;
            contract.Notes = updateDto.Notes;
            contract.ManagerId = updateDto.ManagerId;
            contract.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Smazání pojistné smlouvy
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteInsuranceContract(int id)
        {
            var contract = await _context.InsuranceContracts
                .Include(s => s.InsuranceClaims)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (contract == null)
                return NotFound();

            if (contract.InsuranceClaims.Any())
                return BadRequest("Nelze smazat pojistnou smlouvu, která má pojistné události");

            _context.InsuranceContracts.Remove(contract);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Získání pojistných událostí smlouvy
        /// </summary>
        [HttpGet("{id}/claims")]
        public async Task<ActionResult<IEnumerable<InsuranceClaimDto>>> GetContractClaims(int id)
        {
            var contract = await _context.InsuranceContracts.FindAsync(id);
            if (contract == null)
                return NotFound();

            var claims = await _context.InsuranceClaims
                .Include(u => u.Adjuster)
                .Include(u => u.Reporter)
                .Where(u => u.InsuranceContractId == id)
                .Select(u => new InsuranceClaimDto
                {
                    Id = u.Id,
                    IncidentDateTime = u.IncidentDateTime,
                    DamageDescription = u.DamageDescription,
                    IncidentLocation = u.IncidentLocation,
                    Witnesses = u.Witnesses,
                    EstimatedDamage = u.EstimatedDamage,
                    MonetaryReserve = u.MonetaryReserve,
                    PaymentAmount = u.PaymentAmount,
                    InsuranceCompanyNumber = u.InsuranceCompanyNumber,
                    ClaimStatus = u.Status,
                    StatusText = u.Status.ToString(),
                    AdjusterNotes = u.AdjusterNotes,
                    ReportedAt = u.ReportedAt,
                    ResolvedAt = u.ResolvedAt,
                    InsuranceContractId = u.InsuranceContractId,
                    AdjusterId = u.AdjusterId,
                    AdjusterName = u.Adjuster != null ? $"{u.Adjuster.FirstName} {u.Adjuster.LastName}" : null,
                    ReporterId = u.ReporterId,
                    ReporterName = u.Reporter != null ? $"{u.Reporter.FirstName} {u.Reporter.LastName}" : null,
                    DaysSinceReported = (DateTime.Now - u.ReportedAt).Days,
                    IsResolved = u.Status == ClaimStatus.Resolved
                })
                .ToListAsync();

            return Ok(claims);
        }

        private async Task<string> GenerateContractNumber()
        {
            var year = DateTime.Now.Year;
            var prefix = $"PS{year}";
            
            var lastNumber = await _context.InsuranceContracts
                .Where(s => s.ContractNumber.StartsWith(prefix))
                .CountAsync();

            return $"{prefix}{(lastNumber + 1):D6}";
        }
    }
}