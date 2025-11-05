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
    public class InsuredPersonsController : ControllerBase
    {
        private readonly InsuranceDbContext _context;

        public InsuredPersonsController(InsuranceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Získání seznamu pojištěnců s možností vyhledávání a stránkování
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<InsuredPersonDto>>> GetInsuredPersons([FromQuery] InsuredPersonSearchDto search)
        {
            var query = _context.InsuredPersons.AsQueryable();

            // Filtrování
            if (!string.IsNullOrEmpty(search.FirstName))
                query = query.Where(p => p.FirstName.Contains(search.FirstName));

            if (!string.IsNullOrEmpty(search.LastName))
                query = query.Where(p => p.LastName.Contains(search.LastName));

            if (!string.IsNullOrEmpty(search.Email))
                query = query.Where(p => p.Email != null && p.Email.Contains(search.Email));

            if (!string.IsNullOrEmpty(search.PersonalNumber))
                query = query.Where(p => p.NationalId != null && p.NationalId.Contains(search.PersonalNumber));

            if (search.IsActive.HasValue)
                query = query.Where(p => p.IsActive == search.IsActive.Value);

            var totalCount = await query.CountAsync();

            var pojistenci = await query
                .Include(p => p.InsuranceContracts)
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .Skip((search.Page - 1) * search.PageSize)
                .Take(search.PageSize)
                .Select(p => new InsuredPersonDto
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    DateOfBirth = p.DateOfBirth,
                    Phone = p.Phone,
                    Email = p.Email,
                    Address = p.Address,
                    PersonalNumber = p.NationalId,
                    IdCardNumber = p.IdCardNumber,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt,
                    ContractCount = p.InsuranceContracts.Count
                })
                .ToListAsync();

            return Ok(new PagedResult<InsuredPersonDto>
            {
                Items = pojistenci,
                TotalCount = totalCount,
                Page = search.Page,
                PageSize = search.PageSize
            });
        }

        /// <summary>
        /// Získání detailu pojištěnce podle ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<InsuredPersonDto>> GetInsuredPerson(int id)
        {
            var pojistenec = await _context.InsuredPersons
                .Include(p => p.InsuranceContracts)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pojistenec == null)
                return NotFound();

            var dto = new InsuredPersonDto
            {
                Id = pojistenec.Id,
                FirstName = pojistenec.FirstName,
                LastName = pojistenec.LastName,
                DateOfBirth = pojistenec.DateOfBirth,
                Phone = pojistenec.Phone,
                Email = pojistenec.Email,
                Address = pojistenec.Address,
                PersonalNumber = pojistenec.NationalId,
                IdCardNumber = pojistenec.IdCardNumber,
                IsActive = pojistenec.IsActive,
                CreatedAt = pojistenec.CreatedAt,
                ContractCount = pojistenec.InsuranceContracts.Count
            };

            return Ok(dto);
        }

        /// <summary>
        /// Vytvoření nového pojištěnce
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Broker")]
        public async Task<ActionResult<InsuredPersonDto>> CreateInsuredPerson([FromBody] CreateInsuredPersonDto createDto)
        {
            // Kontrola duplicity rodného čísla
            if (!string.IsNullOrEmpty(createDto.PersonalNumber))
            {
                var existingByRodneCislo = await _context.InsuredPersons
                    .AnyAsync(p => p.NationalId == createDto.PersonalNumber);
                if (existingByRodneCislo)
                    return BadRequest("An insured person with this personal number already exists");
            }

            var pojistenec = new InsuredPerson
            {
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                DateOfBirth = createDto.DateOfBirth,
                Phone = createDto.Phone,
                Email = createDto.Email,
                Address = createDto.Address,
                NationalId = createDto.PersonalNumber,
                IdCardNumber = createDto.IdCardNumber,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.InsuredPersons.Add(pojistenec);
            await _context.SaveChangesAsync();

            var dto = new InsuredPersonDto
            {
                Id = pojistenec.Id,
                FirstName = pojistenec.FirstName,
                LastName = pojistenec.LastName,
                DateOfBirth = pojistenec.DateOfBirth,
                Phone = pojistenec.Phone,
                Email = pojistenec.Email,
                Address = pojistenec.Address,
                PersonalNumber = pojistenec.NationalId,
                IdCardNumber = pojistenec.IdCardNumber,
                IsActive = pojistenec.IsActive,
                CreatedAt = pojistenec.CreatedAt,
                ContractCount = 0
            };

            return CreatedAtAction(nameof(GetInsuredPerson), new { id = pojistenec.Id }, dto);
        }

        /// <summary>
        /// Aktualizace pojištěnce
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Broker")]
        public async Task<IActionResult> UpdateInsuredPerson(int id, [FromBody] UpdateInsuredPersonDto updateDto)
        {
            var pojistenec = await _context.InsuredPersons.FindAsync(id);
            if (pojistenec == null)
                return NotFound();

            // Kontrola duplicity rodného čísla
            if (!string.IsNullOrEmpty(updateDto.PersonalNumber) && updateDto.PersonalNumber != pojistenec.NationalId)
            {
                var existingByRodneCislo = await _context.InsuredPersons
                    .AnyAsync(p => p.NationalId == updateDto.PersonalNumber && p.Id != id);
                if (existingByRodneCislo)
                    return BadRequest("An insured person with this personal number already exists");
            }

            pojistenec.FirstName = updateDto.FirstName;
            pojistenec.LastName = updateDto.LastName;
            pojistenec.DateOfBirth = updateDto.DateOfBirth;
            pojistenec.Phone = updateDto.Phone;
            pojistenec.Email = updateDto.Email;
            pojistenec.Address = updateDto.Address;
            pojistenec.NationalId = updateDto.PersonalNumber;
            pojistenec.IdCardNumber = updateDto.IdCardNumber;
            pojistenec.IsActive = updateDto.IsActive;
            pojistenec.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Smazání pojištěnce
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteInsuredPerson(int id)
        {
            var pojistenec = await _context.InsuredPersons
                .Include(p => p.InsuranceContracts)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pojistenec == null)
                return NotFound();

            if (pojistenec.InsuranceContracts.Any())
                return BadRequest("Cannot delete an insured person who has active insurance contracts");

            _context.InsuredPersons.Remove(pojistenec);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Získání pojistných smluv pojištěnce
        /// </summary>
        [HttpGet("{id}/contracts")]
        public async Task<ActionResult<IEnumerable<InsuranceContractDto>>> GetInsuredPersonContracts(int id)
        {
            var pojistenec = await _context.InsuredPersons.FindAsync(id);
            if (pojistenec == null)
                return NotFound();

            var smlouvy = await _context.InsuranceContracts
                .Include(s => s.Manager)
                .Include(s => s.InsuranceClaims)
                .Where(s => s.InsuredPersonId == id)
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
                    InsuredPersonName = $"{pojistenec.FirstName} {pojistenec.LastName}",
                    ManagerId = s.ManagerId,
                    ManagerName = s.Manager != null ? $"{s.Manager.FirstName} {s.Manager.LastName}" : null,
                    // Compute-only fields set post-query to avoid provider translation issues
                    IsValid = false,
                    DaysToExpiry = 0,
                    ClaimCount = s.InsuranceClaims.Count
                })
                .ToListAsync();

            // Compute fields client-side to avoid translation limitations in MySQL provider
            foreach (var c in smlouvy)
            {
                c.IsValid = DateTime.Now >= c.ValidFrom && DateTime.Now <= c.ValidTo && c.Status == ContractStatus.Active;
                c.DaysToExpiry = (c.ValidTo - DateTime.Now).Days;
            }

            return Ok(smlouvy);
        }
    }
}