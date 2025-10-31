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
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly InsuranceDbContext _context;
        private readonly IFileService _fileService;

        public FilesController(InsuranceDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        /// <summary>
        /// Nahrání souboru k pojistné smlouvě
        /// </summary>
        [HttpPost("contract/{contractId}")]
        [Authorize(Roles = "Admin,Broker")]
        public async Task<ActionResult<FileDto>> UploadContractFile(int contractId, [FromForm] FileUploadDto uploadDto)
        {
            var contract = await _context.InsuranceContracts.FindAsync(contractId);
            if (contract == null)
                return NotFound("Pojistná smlouva neexistuje");

            if (uploadDto.File == null || uploadDto.File.Length == 0)
                return BadRequest("Soubor nebyl vybrán");

            // Validace souboru
            string[] allowedExtensions = { ".pdf", ".docx", ".jpg", ".jpeg", ".png" };
            if (!_fileService.IsValidFileType(uploadDto.File.FileName, allowedExtensions))
                return BadRequest("Nepodporovaný typ souboru");

            long maxSizeInBytes = 10 * 1024 * 1024; // 10 MB
            if (!_fileService.IsValidFileSize(uploadDto.File.Length, maxSizeInBytes))
                return BadRequest("Soubor je příliš velký");

            try
            {
                var filePath = await _fileService.SaveFileAsync(uploadDto.File, "contracts");

                var contractFile = new ContractFile
                {
                    FileName = uploadDto.File.FileName,
                    FilePath = filePath,
                    FileType = Path.GetExtension(uploadDto.File.FileName),
                    FileSize = uploadDto.File.Length,
                    Description = uploadDto.Description,
                    UploadedAt = DateTime.UtcNow,
                    InsuranceContractId = contractId
                };

                _context.ContractFiles.Add(contractFile);
                await _context.SaveChangesAsync();

                var dto = new FileDto
                {
                    Id = contractFile.Id,
                    FileName = contractFile.FileName,
                    FileType = contractFile.FileType,
                    FileTypeText = contractFile.FileType,
                    FileSize = contractFile.FileSize,
                    FileSizeText = FormatFileSize(contractFile.FileSize),
                    Description = contractFile.Description,
                    UploadedAt = contractFile.UploadedAt
                };

                return CreatedAtAction(nameof(DownloadContractFile), new { contractId, fileId = contractFile.Id }, dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Chyba při nahrávání souboru: {ex.Message}");
            }
        }

        /// <summary>
        /// Nahrání souboru k pojistné události
        /// </summary>
        [HttpPost("claim/{claimId}")]
        [Authorize(Roles = "Admin,Broker,Adjuster")]
        public async Task<ActionResult<FileDto>> UploadClaimFile(int claimId, [FromForm] FileUploadDto uploadDto)
        {
            var claim = await _context.InsuranceClaims.FindAsync(claimId);
            if (claim == null)
                return NotFound("Pojistná událost neexistuje");

            if (uploadDto.File == null || uploadDto.File.Length == 0)
                return BadRequest("Soubor nebyl vybrán");

            // Validace souboru
            string[] allowedExtensions = { ".pdf", ".docx", ".jpg", ".jpeg", ".png" };
            if (!_fileService.IsValidFileType(uploadDto.File.FileName, allowedExtensions))
                return BadRequest("Nepodporovaný typ souboru");

            long maxSizeInBytes = 10 * 1024 * 1024; // 10 MB
            if (!_fileService.IsValidFileSize(uploadDto.File.Length, maxSizeInBytes))
                return BadRequest("Soubor je příliš velký");

            try
            {
                var filePath = await _fileService.SaveFileAsync(uploadDto.File, "claims");

                // Map category string to enum
                if (string.IsNullOrWhiteSpace(uploadDto.FileCategory) ||
                    !Enum.TryParse<FileCategory>(uploadDto.FileCategory, true, out var parsedCategory))
                {
                    return BadRequest("Neplatná kategorie souboru. Povolené hodnoty: Document, Photo.");
                }

                var claimFile = new ClaimFile
                {
                    FileName = uploadDto.File.FileName,
                    FilePath = filePath,
                    FileType = Path.GetExtension(uploadDto.File.FileName),
                    FileSize = uploadDto.File.Length,
                    FileCategory = parsedCategory,
                    Description = uploadDto.Description,
                    UploadedAt = DateTime.UtcNow,
                    InsuranceClaimId = claimId
                };

                _context.ClaimFiles.Add(claimFile);
                await _context.SaveChangesAsync();

                var dto = new FileDto
                {
                    Id = claimFile.Id,
                    FileName = claimFile.FileName,
                    FileType = claimFile.FileType,
                    FileTypeText = claimFile.FileType,
                    FileSize = claimFile.FileSize,
                    FileSizeText = FormatFileSize(claimFile.FileSize),
                    FileCategory = claimFile.FileCategory.ToString(),
                    Description = claimFile.Description,
                    UploadedAt = claimFile.UploadedAt
                };

                return CreatedAtAction(nameof(DownloadClaimFile), new { claimId, fileId = claimFile.Id }, dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Chyba při nahrávání souboru: {ex.Message}");
            }
        }

        /// <summary>
        /// Získání seznamu souborů pojistné smlouvy
        /// </summary>
        [HttpGet("contract/{contractId}")]
        [Authorize(Roles = "Admin,Broker")]
        public async Task<ActionResult<IEnumerable<FileDto>>> GetContractFiles(int contractId)
        {
            var contract = await _context.InsuranceContracts.FindAsync(contractId);
            if (contract == null)
                return NotFound("Pojistná smlouva neexistuje");

            var files = await _context.ContractFiles
                .Where(s => s.InsuranceContractId == contractId)
                .Select(s => new FileDto
                {
                    Id = s.Id,
                    FileName = s.FileName,
                    FileType = s.FileType,
                    FileTypeText = s.FileType,
                    FileSize = s.FileSize,
                    FileSizeText = FormatFileSize(s.FileSize),
                    Description = s.Description,
                    UploadedAt = s.UploadedAt
                })
                .ToListAsync();

            return Ok(files);
        }

        /// <summary>
        /// Získání seznamu souborů pojistné události
        /// </summary>
        [HttpGet("claim/{claimId}")]
        [Authorize(Roles = "Admin,Broker,Adjuster")]
        public async Task<ActionResult<IEnumerable<FileDto>>> GetClaimFiles(int claimId)
        {
            var claim = await _context.InsuranceClaims.FindAsync(claimId);
            if (claim == null)
                return NotFound("Pojistná událost neexistuje");

            var files = await _context.ClaimFiles
                .Where(s => s.InsuranceClaimId == claimId)
                .Select(s => new FileDto
                {
                    Id = s.Id,
                    FileName = s.FileName,
                    FileType = s.FileType,
                    FileTypeText = s.FileType,
                    FileSize = s.FileSize,
                    FileSizeText = FormatFileSize(s.FileSize),
                    FileCategory = s.FileCategory.ToString(),
                    Description = s.Description,
                    UploadedAt = s.UploadedAt
                })
                .ToListAsync();

            return Ok(files);
        }

        /// <summary>
        /// Stažení souboru pojistné smlouvy
        /// </summary>
        [HttpGet("contract/{contractId}/download/{fileId}")]
        [Authorize(Roles = "Admin,Broker")]
        public async Task<IActionResult> DownloadContractFile(int contractId, int fileId)
        {
            var file = await _context.ContractFiles
                .FirstOrDefaultAsync(s => s.Id == fileId && s.InsuranceContractId == contractId);

            if (file == null)
                return NotFound("Soubor neexistuje");

            if (!System.IO.File.Exists(file.FilePath))
                return NotFound("Fyzický soubor neexistuje");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(file.FilePath);
            var contentType = _fileService.GetContentType(file.FileName);

            return File(fileBytes, contentType, file.FileName);
        }

        /// <summary>
        /// Stažení souboru pojistné události
        /// </summary>
        [HttpGet("claim/{claimId}/download/{fileId}")]
        [Authorize(Roles = "Admin,Broker,Adjuster")]
        public async Task<IActionResult> DownloadClaimFile(int claimId, int fileId)
        {
            var file = await _context.ClaimFiles
                .FirstOrDefaultAsync(s => s.Id == fileId && s.InsuranceClaimId == claimId);

            if (file == null)
                return NotFound("Soubor neexistuje");

            if (!System.IO.File.Exists(file.FilePath))
                return NotFound("Fyzický soubor neexistuje");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(file.FilePath);
            var contentType = _fileService.GetContentType(file.FileName);

            return File(fileBytes, contentType, file.FileName);
        }

        /// <summary>
        /// Smazání souboru pojistné smlouvy
        /// </summary>
        [HttpDelete("contract/{contractId}/{fileId}")]
        [Authorize(Roles = "Admin,Broker")]
        public async Task<IActionResult> DeleteContractFile(int contractId, int fileId)
        {
            var file = await _context.ContractFiles
                .FirstOrDefaultAsync(s => s.Id == fileId && s.InsuranceContractId == contractId);

            if (file == null)
                return NotFound("Soubor neexistuje");

            try
            {
                // Smazání fyzického souboru
                if (System.IO.File.Exists(file.FilePath))
                {
                    System.IO.File.Delete(file.FilePath);
                }

                // Smazání záznamu z databáze
                _context.ContractFiles.Remove(file);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Chyba při mazání souboru: {ex.Message}");
            }
        }

        /// <summary>
        /// Smazání souboru pojistné události
        /// </summary>
        [HttpDelete("claim/{claimId}/{fileId}")]
        [Authorize(Roles = "Admin,Broker,Adjuster")]
        public async Task<IActionResult> DeleteClaimFile(int claimId, int fileId)
        {
            var file = await _context.ClaimFiles
                .FirstOrDefaultAsync(s => s.Id == fileId && s.InsuranceClaimId == claimId);

            if (file == null)
                return NotFound("Soubor neexistuje");

            try
            {
                // Smazání fyzického souboru
                if (System.IO.File.Exists(file.FilePath))
                {
                    System.IO.File.Delete(file.FilePath);
                }

                // Smazání záznamu z databáze
                _context.ClaimFiles.Remove(file);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Chyba při mazání souboru: {ex.Message}");
            }
        }

        private static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}