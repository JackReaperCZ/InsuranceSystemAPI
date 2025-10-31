using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InsuranceSystemAPI.Services;
using InsuranceSystemAPI.Models;
using System.Security.Claims;

namespace InsuranceSystemAPI.Controllers
{
    /// <summary>
    /// Controller pro GDPR compliance operace
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GdprController : ControllerBase
    {
        private readonly IGdprService _gdprService;
        private readonly ILogger<GdprController> _logger;

        public GdprController(IGdprService gdprService, ILogger<GdprController> logger)
        {
            _gdprService = gdprService;
            _logger = logger;
        }

        /// <summary>
        /// Export všech osobních dat pojištěnce
        /// </summary>
        [HttpGet("export/{insuredPersonId}")]
        [Authorize(Roles = "Admin,Makler")]
        public async Task<ActionResult<GdprDataExport>> ExportPersonalData(int insuredPersonId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var export = await _gdprService.ExportPersonalDataAsync(insuredPersonId, userId);
                
                return Ok(export);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při exportu osobních dat pro pojištěnce {InsuredPersonId}", insuredPersonId);
                return StatusCode(500, "Chyba při exportu osobních dat");
            }
        }

        /// <summary>
        /// Anonymizace osobních dat pojištěnce
        /// </summary>
        [HttpPost("anonymize/{insuredPersonId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AnonymizePersonalData(int insuredPersonId, [FromBody] AnonymizeRequestDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                if (!await _gdprService.CanAnonymizeAsync(insuredPersonId))
                {
                    return BadRequest("Pojištěnce nelze anonymizovat - má aktivní smlouvy nebo nevyřešené události");
                }

                var success = await _gdprService.AnonymizePersonalDataAsync(insuredPersonId, request.Reason, userId);
                
                if (success)
                {
                    return Ok(new { message = "Osobní data byla úspěšně anonymizována" });
                }
                else
                {
                    return NotFound("Pojištěnec nebyl nalezen");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při anonymizaci dat pro pojištěnce {InsuredPersonId}", insuredPersonId);
                return StatusCode(500, "Chyba při anonymizaci osobních dat");
            }
        }

        /// <summary>
        /// Kontrola možnosti anonymizace
        /// </summary>
        [HttpGet("can-anonymize/{insuredPersonId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> CanAnonymize(int insuredPersonId)
        {
            try
            {
                var canAnonymize = await _gdprService.CanAnonymizeAsync(insuredPersonId);
                return Ok(canAnonymize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při kontrole možnosti anonymizace pro pojištěnce {InsuredPersonId}", insuredPersonId);
                return StatusCode(500, "Chyba při kontrole možnosti anonymizace");
            }
        }

        /// <summary>
        /// Získání audit logu pro pojištěnce
        /// </summary>
        [HttpGet("audit-log/{insuredPersonId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<GdprAuditLog>>> GetAuditLog(
            int insuredPersonId, 
            [FromQuery] DateTime? fromDate = null, 
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var auditLog = await _gdprService.GetAuditLogAsync(insuredPersonId);
                return Ok(auditLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při získávání audit logu pro pojištěnce {InsuredPersonId}", insuredPersonId);
                return StatusCode(500, "Chyba při získávání audit logu");
            }
        }

        /// <summary>
        /// Zaznamenání souhlasu se zpracováním osobních údajů
        /// </summary>
        [HttpPost("consent/{insuredPersonId}")]
        [Authorize(Roles = "Admin,Makler")]
        public async Task<ActionResult> RecordConsent(int insuredPersonId, [FromBody] ConsentRequestDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                
                await _gdprService.RecordConsentAsync(
                    insuredPersonId, 
                    request.Category, 
                    request.Purpose, 
                    userId);
                
                return Ok(new { message = "Souhlas byl úspěšně zaznamenán" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při zaznamenávání souhlasu pro pojištěnce {InsuredPersonId}", insuredPersonId);
                return StatusCode(500, "Chyba při zaznamenávání souhlasu");
            }
        }

        /// <summary>
        /// Odvolání souhlasu se zpracováním osobních údajů
        /// </summary>
        [HttpDelete("consent/{insuredPersonId}")]
        [Authorize(Roles = "Admin,Makler")]
        public async Task<ActionResult> RevokeConsent(int insuredPersonId, [FromBody] RevokeConsentRequestDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                await _gdprService.RevokeConsentAsync(
                    insuredPersonId, 
                    request.Category, 
                    userId);
                
                return Ok(new { message = "Souhlas byl úspěšně odvolán" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při odvolávání souhlasu pro pojištěnce {InsuredPersonId}", insuredPersonId);
                return StatusCode(500, "Chyba při odvolávání souhlasu");
            }
        }

        /// <summary>
        /// Kontrola platnosti souhlasu
        /// </summary>
        [HttpGet("consent/{insuredPersonId}/check")]
        [Authorize(Roles = "Admin,Makler")]
        public async Task<ActionResult<bool>> CheckConsent(int insuredPersonId, [FromQuery] PersonalDataCategory category)
        {
            try
            {
                var hasConsent = await _gdprService.HasValidConsentAsync(insuredPersonId, category);
                return Ok(hasConsent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při kontrole souhlasu pro pojištěnce {InsuredPersonId}", insuredPersonId);
                return StatusCode(500, "Chyba při kontrole souhlasu");
            }
        }

        /// <summary>
        /// Získání aktuálního ID uživatele z JWT tokenu
        /// </summary>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("Neplatný uživatelský token");
        }
    }

    /// <summary>
    /// DTO pro požadavek na anonymizaci
    /// </summary>
    public class AnonymizeRequestDto
    {
        public string Reason { get; set; } = "";
    }

    /// <summary>
    /// DTO pro požadavek na zaznamenání souhlasu
    /// </summary>
    public class ConsentRequestDto
    {
        public PersonalDataCategory Category { get; set; }
        public string Purpose { get; set; } = "";
    }

    /// <summary>
    /// DTO pro požadavek na odvolání souhlasu
    /// </summary>
    public class RevokeConsentRequestDto
    {
        public PersonalDataCategory Category { get; set; }
        public string Reason { get; set; } = "";
    }
}