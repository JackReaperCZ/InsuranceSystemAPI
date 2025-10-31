using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using InsuranceSystemAPI.DTOs;
using InsuranceSystemAPI.Services;

namespace InsuranceSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Přihlášení uživatele
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto loginRequest)
        {
            var result = await _authService.LoginAsync(loginRequest);
            
            if (result == null)
            {
                return Unauthorized(new { message = "Neplatné přihlašovací údaje" });
            }

            return Ok(result);
        }

        /// <summary>
        /// Registrace nového uživatele (pouze pro adminy)
        /// </summary>
        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterRequestDto registerRequest)
        {
            var result = await _authService.RegisterAsync(registerRequest);
            
            if (result == null)
            {
                return BadRequest(new { message = "Uživatel s tímto jménem nebo emailem již existuje" });
            }

            return CreatedAtAction(nameof(GetProfile), new { id = result.Id }, result);
        }

        /// <summary>
        /// Získání profilu přihlášeného uživatele
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _authService.GetUserByIdAsync(userId);
            
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        /// <summary>
        /// Změna hesla
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);
            
            if (!result)
            {
                return BadRequest(new { message = "Neplatné staré heslo" });
            }

            return Ok(new { message = "Heslo bylo úspěšně změněno" });
        }

        /// <summary>
        /// Ověření platnosti tokenu
        /// </summary>
        [HttpGet("verify")]
        [Authorize]
        public async Task<IActionResult> VerifyToken()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isActive = await _authService.IsUserActiveAsync(userId);
            
            if (!isActive)
            {
                return Unauthorized(new { message = "Uživatel není aktivní" });
            }

            return Ok(new { message = "Token je platný", userId });
        }
    }
}