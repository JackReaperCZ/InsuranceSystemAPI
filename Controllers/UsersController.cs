using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InsuranceSystemAPI.Data;
using InsuranceSystemAPI.DTOs;
using InsuranceSystemAPI.Models;
using InsuranceSystemAPI.Services;

namespace InsuranceSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly InsuranceDbContext _context;
        private readonly IAuthService _authService;

        public UsersController(InsuranceDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        /// <summary>
        /// Získání seznamu všech uživatelů
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<UserDto>>> GetUsers([FromQuery] UserSearchDto search)
        {
            var query = _context.Users.AsQueryable();

            // Filtrování
            if (!string.IsNullOrEmpty(search.Username))
                query = query.Where(u => u.Username.Contains(search.Username));

            if (!string.IsNullOrEmpty(search.FirstName))
                query = query.Where(u => u.FirstName.Contains(search.FirstName));

            if (!string.IsNullOrEmpty(search.LastName))
                query = query.Where(u => u.LastName.Contains(search.LastName));

            if (!string.IsNullOrEmpty(search.Email))
                query = query.Where(u => u.Email != null && u.Email.Contains(search.Email));

            if (search.Role.HasValue)
                query = query.Where(u => u.Role == search.Role.Value);

            if (search.IsActive.HasValue)
                query = query.Where(u => u.IsActive == search.IsActive.Value);

            var totalCount = await query.CountAsync();

            var uzivatele = await query
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Skip((search.Page - 1) * search.PageSize)
                .Take(search.PageSize)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Role = u.Role,
                    RoleText = u.Role.ToString(),
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    LastLogin = u.LastLogin
                })
                .ToListAsync();

            return Ok(new PagedResult<UserDto>
            {
                Items = uzivatele,
                TotalCount = totalCount,
                Page = search.Page,
                PageSize = search.PageSize
            });
        }

        /// <summary>
        /// Získání detailu uživatele podle ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var uzivatel = await _context.Users.FindAsync(id);
            if (uzivatel == null)
                return NotFound();

            var dto = new UserDto
            {
                Id = uzivatel.Id,
                Username = uzivatel.Username,
                FirstName = uzivatel.FirstName,
                LastName = uzivatel.LastName,
                Email = uzivatel.Email,
                Phone = uzivatel.Phone,
                Role = uzivatel.Role,
                RoleText = uzivatel.Role.ToString(),
                IsActive = uzivatel.IsActive,
                CreatedAt = uzivatel.CreatedAt,
                LastLogin = uzivatel.LastLogin
            };

            return Ok(dto);
        }

        /// <summary>
        /// Vytvoření nového uživatele
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] RegisterRequestDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            if (result == null)
                return BadRequest("A user with this username or email already exists.");

            return Ok(result);
        }

        /// <summary>
        /// Aktualizace uživatele
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateDto)
        {
            var uzivatel = await _context.Users.FindAsync(id);
            if (uzivatel == null)
                return NotFound();

            // Kontrola duplicity uživatelského jména
            if (updateDto.Username != uzivatel.Username)
            {
                var existingByUsername = await _context.Users
                    .AnyAsync(u => u.Username == updateDto.Username && u.Id != id);
                if (existingByUsername)
                    return BadRequest("Username already exists");
            }

            // Kontrola duplicity emailu
            if (!string.IsNullOrEmpty(updateDto.Email) && updateDto.Email != uzivatel.Email)
            {
                var existingByEmail = await _context.Users
                    .AnyAsync(u => u.Email == updateDto.Email && u.Id != id);
                if (existingByEmail)
                    return BadRequest("Email already exists");
            }

            uzivatel.Username = updateDto.Username;
            uzivatel.FirstName = updateDto.FirstName;
            uzivatel.LastName = updateDto.LastName;
            uzivatel.Email = updateDto.Email ?? uzivatel.Email;
            uzivatel.Phone = updateDto.Phone;
            uzivatel.Role = updateDto.Role;
            uzivatel.IsActive = updateDto.IsActive;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Smazání uživatele
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var uzivatel = await _context.Users
                .Include(u => u.ManagedContracts)
                .Include(u => u.ProcessedClaims)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (uzivatel == null)
                return NotFound();

            if (uzivatel.ManagedContracts.Any() || uzivatel.ProcessedClaims.Any())
                return BadRequest("Cannot delete a user who has assigned contracts or claims");

            _context.Users.Remove(uzivatel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Aktivace/deaktivace uživatele
        /// </summary>
        [HttpPut("{id}/activation")]
        public async Task<IActionResult> ToggleUserActivation(int id)
        {
            var uzivatel = await _context.Users.FindAsync(id);
            if (uzivatel == null)
                return NotFound();

            uzivatel.IsActive = !uzivatel.IsActive;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Reset hesla uživatele
        /// </summary>
        [HttpPut("{id}/reset-password")]
        public async Task<ActionResult<string>> ResetPassword(int id)
        {
            var uzivatel = await _context.Users.FindAsync(id);
            if (uzivatel == null)
                return NotFound();

            // Generování nového hesla
            var newPassword = GenerateRandomPassword();
            uzivatel.HashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

            await _context.SaveChangesAsync();

            return Ok(new { NewPassword = newPassword });
        }

        /// <summary>
        /// Získání statistik uživatele
        /// </summary>
        [HttpGet("{id}/statistics")]
        public async Task<ActionResult<UserStatisticsDto>> GetUserStatistics(int id)
        {
            var uzivatel = await _context.Users.FindAsync(id);
            if (uzivatel == null)
                return NotFound();

            var statistiky = new UserStatisticsDto
            {
                ManagedContractsCount = await _context.InsuranceContracts
                    .CountAsync(s => s.ManagerId == id),
                ProcessedClaimsCount = await _context.InsuranceClaims
                    .CountAsync(u => u.AdjusterId == id && u.Status == ClaimStatus.Resolved),
                PendingClaimsCount = await _context.InsuranceClaims
                    .CountAsync(u => u.AdjusterId == id && u.Status != ClaimStatus.Resolved),
                TotalPaymentAmount = await _context.InsuranceClaims
                    .Where(u => u.AdjusterId == id)
                    .SumAsync(u => u.PaymentAmount ?? 0)
            };

            return Ok(statistiky);
        }

        /// <summary>
        /// Získání seznamu makléřů pro dropdown
        /// </summary>
        [HttpGet("brokers")]
        [Authorize(Roles = "Admin,Broker")]
        public async Task<ActionResult<IEnumerable<UserDropdownDto>>> GetBrokers()
        {
            var makleri = await _context.Users
                .Where(u => u.Role == UserRole.Broker && u.IsActive)
                .Select(u => new UserDropdownDto
                {
                    Id = u.Id,
                    FullName = $"{u.FirstName} {u.LastName}",
                    Username = u.Username
                })
                .OrderBy(u => u.FullName)
                .ToListAsync();

            return Ok(makleri);
        }

        /// <summary>
        /// Získání seznamu likvidátorů pro dropdown
        /// </summary>
        [HttpGet("adjusters")]
        [Authorize(Roles = "Admin,Broker,Adjuster")]
        public async Task<ActionResult<IEnumerable<UserDropdownDto>>> GetAdjusters()
        {
            var likvidatori = await _context.Users
                .Where(u => u.Role == UserRole.Adjuster && u.IsActive)
                .Select(u => new UserDropdownDto
                {
                    Id = u.Id,
                    FullName = $"{u.FirstName} {u.LastName}",
                    Username = u.Username
                })
                .OrderBy(u => u.FullName)
                .ToListAsync();

            return Ok(likvidatori);
        }

        private static string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

    public class UpdateUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserSearchDto
    {
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public UserRole? Role { get; set; }
        public bool? IsActive { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class UserStatisticsDto
    {
        public int ManagedContractsCount { get; set; }
        public int ProcessedClaimsCount { get; set; }
        public int PendingClaimsCount { get; set; }
        public decimal TotalPaymentAmount { get; set; }
    }

    public class UserDropdownDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}