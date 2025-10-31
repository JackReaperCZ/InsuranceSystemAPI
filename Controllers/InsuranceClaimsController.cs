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
    public class InsuranceClaimsController : ControllerBase
    {
        private readonly InsuranceDbContext _context;

        public InsuranceClaimsController(InsuranceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Získání seznamu pojistných událostí s filtrováním
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Adjuster,Broker")]
        public async Task<ActionResult<IEnumerable<InsuranceClaimDto>>> GetInsuranceClaims([FromQuery] InsuranceClaimSearchDto search)
        {
            var query = _context.InsuranceClaims
                .Include(u => u.InsuranceContract)
                    .ThenInclude(s => s.InsuredPerson)
                .Include(u => u.Adjuster)
                .Include(u => u.Reporter)
                .Include(u => u.AttachedFiles)
                .AsQueryable();

            // Filtrování
            if (!string.IsNullOrEmpty(search.InsuranceCompanyNumber))
                query = query.Where(u => (u.InsuranceCompanyNumber ?? string.Empty).Contains(search.InsuranceCompanyNumber));

            if (search.ClaimStatus.HasValue)
                query = query.Where(u => u.Status == search.ClaimStatus.Value);

            if (search.IncidentDateFrom.HasValue)
                query = query.Where(u => u.IncidentDateTime >= search.IncidentDateFrom.Value);

            if (search.IncidentDateTo.HasValue)
                query = query.Where(u => u.IncidentDateTime <= search.IncidentDateTo.Value);

            if (search.ReportedDateFrom.HasValue)
                query = query.Where(u => u.ReportedAt >= search.ReportedDateFrom.Value);

            if (search.ReportedDateTo.HasValue)
                query = query.Where(u => u.ReportedAt <= search.ReportedDateTo.Value);

            if (search.InsuranceContractId.HasValue)
                query = query.Where(u => u.InsuranceContractId == search.InsuranceContractId.Value);

            if (search.AdjusterId.HasValue)
                query = query.Where(u => u.AdjusterId == search.AdjusterId.Value);

            if (!string.IsNullOrEmpty(search.IncidentLocation))
                query = query.Where(u => u.IncidentLocation.Contains(search.IncidentLocation));

            if (!string.IsNullOrEmpty(search.InsuredPersonName))
                query = query.Where(u => (
                    (u.InsuranceContract != null && u.InsuranceContract.InsuredPerson != null)
                        ? (u.InsuranceContract.InsuredPerson.FirstName + " " + u.InsuranceContract.InsuredPerson.LastName)
                        : string.Empty
                ).Contains(search.InsuredPersonName));

            var claimEntities = await query
                .OrderByDescending(u => u.ReportedAt)
                .ToListAsync();

            var claims = claimEntities
                .Select(u => new InsuranceClaimDto
                {
                    Id = u.Id,
                    InsuranceCompanyNumber = u.InsuranceCompanyNumber,
                    IncidentDateTime = u.IncidentDateTime,
                    DamageDescription = u.DamageDescription,
                    IncidentLocation = u.IncidentLocation,
                    Witnesses = u.Witnesses,
                    EstimatedDamage = u.EstimatedDamage,
                    MonetaryReserve = u.MonetaryReserve,
                    PaymentAmount = u.PaymentAmount,
                    ClaimStatus = u.Status,
                    StatusText = u.Status.ToString(),
                    AdjusterNotes = u.AdjusterNotes,
                    ReportedAt = u.ReportedAt,
                    ResolvedAt = u.ResolvedAt,
                    InsuranceContractId = u.InsuranceContractId,
                    ContractNumber = u.InsuranceContract != null ? u.InsuranceContract.ContractNumber : string.Empty,
                    InsuranceContractNumber = u.InsuranceContract != null ? u.InsuranceContract.ContractNumber : string.Empty,
                    InsuredPersonName = (u.InsuranceContract != null && u.InsuranceContract.InsuredPerson != null)
                        ? ($"{u.InsuranceContract.InsuredPerson.FirstName} {u.InsuranceContract.InsuredPerson.LastName}")
                        : string.Empty,
                    AdjusterId = u.AdjusterId,
                    AdjusterName = u.Adjuster != null ? ($"{u.Adjuster.FirstName} {u.Adjuster.LastName}") : null,
                    ReporterId = u.ReporterId,
                    ReporterName = u.Reporter != null ? ($"{u.Reporter.FirstName} {u.Reporter.LastName}") : null,
                    DaysSinceReported = (DateTime.Now - u.ReportedAt).Days,
                    IsResolved = u.Status == ClaimStatus.Resolved,
                    FileCount = (u.AttachedFiles != null ? u.AttachedFiles.Count : 0)
                })
                .ToList();

            return Ok(claims);
        }

        /// <summary>
        /// Získání konkrétní pojistné události
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Adjuster,Broker")]
        public async Task<ActionResult<InsuranceClaimDto>> GetInsuranceClaim(int id)
        {
            var claimWithDetails = await _context.InsuranceClaims
                .Include(u => u.InsuranceContract)
                    .ThenInclude(s => s.InsuredPerson)
                .Include(u => u.Adjuster)
                .Include(u => u.Reporter)
                .Include(u => u.AttachedFiles)
                .FirstOrDefaultAsync(u => u.Id == id);
            
            if (claimWithDetails == null)
                return NotFound();

            if (claimWithDetails == null)
                return Problem("Claim could not be loaded after creation.");

            var dto = new InsuranceClaimDto
            {
                Id = claimWithDetails.Id,
                InsuranceCompanyNumber = claimWithDetails.InsuranceCompanyNumber,
                IncidentDateTime = claimWithDetails.IncidentDateTime,
                DamageDescription = claimWithDetails.DamageDescription,
                IncidentLocation = claimWithDetails.IncidentLocation,
                Witnesses = claimWithDetails.Witnesses,
                EstimatedDamage = claimWithDetails.EstimatedDamage,
                MonetaryReserve = claimWithDetails.MonetaryReserve,
                PaymentAmount = claimWithDetails.PaymentAmount,
                ClaimStatus = claimWithDetails.Status,
                StatusText = claimWithDetails.Status.ToString(),
                AdjusterNotes = claimWithDetails.AdjusterNotes,
                ReportedAt = claimWithDetails.ReportedAt,
                ResolvedAt = claimWithDetails.ResolvedAt,
                InsuranceContractId = claimWithDetails.InsuranceContractId,
                ContractNumber = claimWithDetails.InsuranceContract != null ? claimWithDetails.InsuranceContract.ContractNumber : string.Empty,
                InsuranceContractNumber = claimWithDetails.InsuranceContract != null ? claimWithDetails.InsuranceContract.ContractNumber : string.Empty,
                InsuredPersonName = (claimWithDetails.InsuranceContract != null && claimWithDetails.InsuranceContract.InsuredPerson != null)
                    ? $"{claimWithDetails.InsuranceContract.InsuredPerson.FirstName} {claimWithDetails.InsuranceContract.InsuredPerson.LastName}"
                    : string.Empty,
                AdjusterId = claimWithDetails.AdjusterId,
                AdjusterName = claimWithDetails.Adjuster != null ? $"{claimWithDetails.Adjuster.FirstName} {claimWithDetails.Adjuster.LastName}" : null,
                ReporterId = claimWithDetails.ReporterId,
                ReporterName = claimWithDetails.Reporter != null ? $"{claimWithDetails.Reporter.FirstName} {claimWithDetails.Reporter.LastName}" : null,
                DaysSinceReported = (DateTime.Now - claimWithDetails.ReportedAt).Days,
                IsResolved = claimWithDetails.Status == ClaimStatus.Resolved,
                FileCount = claimWithDetails.AttachedFiles.Count
            };

            return Ok(dto);
        }

        /// <summary>
        /// Vytvoření nové pojistné události
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Broker")]
        public async Task<ActionResult<InsuranceClaimDto>> CreateInsuranceClaim([FromBody] CreateInsuranceClaimDto createDto)
        {
            // Kontrola existence smlouvy
            var contract = await _context.InsuranceContracts
                .Include(s => s.InsuredPerson)
                .FirstOrDefaultAsync(s => s.Id == createDto.InsuranceContractId);

            if (contract == null)
                return BadRequest("Pojistná smlouva neexistuje");

            // Kontrola platnosti smlouvy
            if (contract.Status != ContractStatus.Active || 
                contract.ValidFrom > DateTime.Now || 
                contract.ValidTo < DateTime.Now)
            {
                return BadRequest("Pojistná smlouva není aktivní nebo není v platnosti");
            }

            // Kontrola existence nahlašovatele
            var reporter = await _context.Users.FindAsync(createDto.ReporterId);
            if (reporter == null)
                return BadRequest("Nahlašovatel neexistuje");

            var insuranceCompanyNumber = await GenerateInsuranceCompanyNumber();

            var claim = new InsuranceClaim
            {
                InsuranceCompanyNumber = insuranceCompanyNumber,
                IncidentDateTime = createDto.IncidentDateTime,
                DamageDescription = createDto.DamageDescription,
                IncidentLocation = createDto.IncidentLocation,
                Witnesses = createDto.Witnesses,
                EstimatedDamage = createDto.EstimatedDamage,
                MonetaryReserve = createDto.MonetaryReserve,
                Status = ClaimStatus.Reported,
                ReportedAt = DateTime.UtcNow,
                InsuranceContractId = createDto.InsuranceContractId,
                ReporterId = createDto.ReporterId
            };

            _context.InsuranceClaims.Add(claim);
            await _context.SaveChangesAsync();

            // Načtení kompletních dat pro DTO
            var claimWithDetails = await _context.InsuranceClaims
                .Include(u => u.InsuranceContract)
                    .ThenInclude(s => s.InsuredPerson)
                .Include(u => u.Adjuster)
                .Include(u => u.Reporter)
                .FirstOrDefaultAsync(u => u.Id == claim.Id);
            if (claimWithDetails == null)
                return Problem("Claim could not be loaded after creation.");

            var dto = new InsuranceClaimDto
            {
                Id = claimWithDetails.Id,
                InsuranceCompanyNumber = claimWithDetails.InsuranceCompanyNumber,
                IncidentDateTime = claimWithDetails.IncidentDateTime,
                DamageDescription = claimWithDetails.DamageDescription,
                IncidentLocation = claimWithDetails.IncidentLocation,
                Witnesses = claimWithDetails.Witnesses,
                EstimatedDamage = claimWithDetails.EstimatedDamage,
                MonetaryReserve = claimWithDetails.MonetaryReserve,
                PaymentAmount = claimWithDetails.PaymentAmount,
                ClaimStatus = claimWithDetails.Status,
                StatusText = claimWithDetails.Status.ToString(),
                AdjusterNotes = claimWithDetails.AdjusterNotes,
                ReportedAt = claimWithDetails.ReportedAt,
                ResolvedAt = claimWithDetails.ResolvedAt,
                InsuranceContractId = claimWithDetails.InsuranceContractId,
                InsuranceContractNumber = claimWithDetails.InsuranceContract != null ? claimWithDetails.InsuranceContract.ContractNumber : string.Empty,
                ContractNumber = claimWithDetails.InsuranceContract != null ? claimWithDetails.InsuranceContract.ContractNumber : string.Empty,
                InsuredPersonName = (claimWithDetails.InsuranceContract != null && claimWithDetails.InsuranceContract.InsuredPerson != null)
                    ? $"{claimWithDetails.InsuranceContract.InsuredPerson.FirstName} {claimWithDetails.InsuranceContract.InsuredPerson.LastName}"
                    : string.Empty,
                AdjusterId = claimWithDetails.AdjusterId,
                AdjusterName = claimWithDetails.Adjuster != null ? $"{claimWithDetails.Adjuster.FirstName} {claimWithDetails.Adjuster.LastName}" : null,
                ReporterId = claimWithDetails.ReporterId,
                ReporterName = claimWithDetails.Reporter != null ? $"{claimWithDetails.Reporter.FirstName} {claimWithDetails.Reporter.LastName}" : null,
                DaysSinceReported = (DateTime.Now - claimWithDetails.ReportedAt).Days,
                IsResolved = claimWithDetails.Status == ClaimStatus.Resolved
            };

            return CreatedAtAction(nameof(GetInsuranceClaim), new { id = claim.Id }, dto);
        }

        /// <summary>
        /// Aktualizace pojistné události
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Adjuster")]
        public async Task<IActionResult> UpdateInsuranceClaim(int id, [FromBody] UpdateInsuranceClaimDto updateDto)
        {
            var claim = await _context.InsuranceClaims.FindAsync(id);
            if (claim == null)
                return NotFound();

            // Kontrola existence vyřizovatel
            if (updateDto.AdjusterId.HasValue)
            {
                var adjuster = await _context.Users.FindAsync(updateDto.AdjusterId.Value);
                if (adjuster == null)
                    return BadRequest("Zadaný likvidátor neexistuje");
            }

            // Aktualizace vlastností
            claim.IncidentDateTime = updateDto.IncidentDateTime;
            claim.DamageDescription = updateDto.DamageDescription;
            claim.IncidentLocation = updateDto.IncidentLocation;
            claim.Witnesses = updateDto.Witnesses;
            claim.EstimatedDamage = updateDto.EstimatedDamage;
            claim.MonetaryReserve = updateDto.MonetaryReserve;
            claim.PaymentAmount = updateDto.PaymentAmount;
            claim.InsuranceCompanyNumber = updateDto.InsuranceCompanyNumber;
            claim.AdjusterNotes = updateDto.AdjusterNotes;
            claim.AdjusterId = updateDto.AdjusterId;

            // Pokud se mění status na vyřízený, nastavit datum vyřízení
            if (updateDto.ClaimStatus == ClaimStatus.Resolved && claim.ResolvedAt == null)
            {
                claim.ResolvedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Smazání pojistné události
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteInsuranceClaim(int id)
        {
            var claim = await _context.InsuranceClaims.FindAsync(id);
            if (claim == null)
                return NotFound();

            _context.InsuranceClaims.Remove(claim);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Přiřazení likvidátora k události
        /// </summary>
        [HttpPost("{id}/assign-adjuster")]
        [Authorize(Roles = "Admin,Adjuster")]
        public async Task<IActionResult> AssignAdjuster(int id, [FromBody] int adjusterId)
        {
            var claim = await _context.InsuranceClaims.FindAsync(id);
            if (claim == null)
                return NotFound();

            var adjuster = await _context.Users.FindAsync(adjusterId);
            if (adjuster == null || adjuster.Role != UserRole.Adjuster)
                return BadRequest("Neplatný likvidátor");

            claim.AdjusterId = adjusterId;
            claim.Status = ClaimStatus.InProgress;
            claim.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Vyřízení události
        /// </summary>
        [HttpPut("{id}/process")]
        [Authorize(Roles = "Admin,Adjuster")]
        public async Task<IActionResult> ProcessClaim(int id, [FromBody] ProcessClaimDto processDto)
        {
            var claim = await _context.InsuranceClaims.FindAsync(id);
            if (claim == null)
                return NotFound();

            claim.PaymentAmount = processDto.PaymentAmount;
            claim.AdjusterNotes = processDto.AdjusterNotes;
            claim.Status = ClaimStatus.Resolved;
            claim.ResolvedAt = DateTime.UtcNow;
            claim.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<string> GenerateInsuranceCompanyNumber()
        {
            var year = DateTime.Now.Year;
            var prefix = $"PU{year}";
            
            var lastNumber = await _context.InsuranceClaims
                .Where(u => u.InsuranceCompanyNumber != null && u.InsuranceCompanyNumber.StartsWith(prefix))
                .CountAsync();

            return $"{prefix}{(lastNumber + 1):D6}";
        }
    }

    public class ProcessClaimDto
    {
        public decimal? PaymentAmount { get; set; }
        public string? AdjusterNotes { get; set; }
    }
}