using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json;
using InsuranceSystemAPI.Data;
using InsuranceSystemAPI.Models;

namespace InsuranceSystemAPI.Services
{
    /// <summary>
    /// Implementace služby pro GDPR compliance
    /// </summary>
    public class GdprService : IGdprService
    {
        private readonly InsuranceDbContext _context;
        private readonly IEncryptionService _encryptionService;
        private readonly ILogger<GdprService> _logger;

        public GdprService(
            InsuranceDbContext context,
            IEncryptionService encryptionService,
            ILogger<GdprService> logger)
        {
            _context = context;
            _encryptionService = encryptionService;
            _logger = logger;
        }

        /// <summary>
        /// Exportuje všechna osobní data pojištěnce
        /// </summary>
        public async Task<GdprDataExport> ExportPersonalDataAsync(int pojistenecId, int requestedByUserId)
        {
            await LogDataAccessAsync(pojistenecId, requestedByUserId, GdprAction.DataExport, "Export osobních dat");

            var pojistenec = await _context.InsuredPersons
                .Include(p => p.InsuranceContracts)
                    .ThenInclude(s => s.InsuranceClaims)
                .Include(p => p.InsuranceContracts)
                    .ThenInclude(s => s.AttachedFiles)
                .Include(p => p.InsuranceClaims)
                    .ThenInclude(u => u.AttachedFiles)
                .FirstOrDefaultAsync(p => p.Id == pojistenecId);

            if (pojistenec == null)
                throw new ArgumentException("Pojištěnec nebyl nalezen");

            var export = new GdprDataExport
            {
                PojistenecId = pojistenecId,
                ExportDate = DateTime.UtcNow,
                PersonalData = ExtractPersonalData(pojistenec),
                Contracts = pojistenec.InsuranceContracts.Select(s => new ContractData
                {
                    Id = s.Id,
                    CisloSmlouvy = s.ContractNumber,
                    TypPojisteni = s.InsuranceType.ToString(),
                    PojistnaCastka = s.InsuredAmount,
                    DatumUzavreni = s.CreatedAt,
                    DatumPlatnostiOd = s.ValidFrom,
                    DatumPlatnostiDo = s.ValidTo,
                    Status = s.Status.ToString(),
                    Events = s.InsuranceClaims.Select(u => new EventData
                    {
                        Id = u.Id,
                        CisloUdalosti = u.ClaimNumber,
                        DatumNahlaseni = u.ReportedAt,
                        DatumUdalosti = u.IncidentDateTime,
                        Popis = u.DamageDescription,
                        VyseSkody = u.EstimatedDamage,
                        VyseNahrady = u.PaymentAmount,
                        Status = u.Status.ToString()
                    }).ToList(),
                    Files = s.AttachedFiles.Select(f => new FileData
                    {
                        Id = f.Id,
                        NazevSouboru = f.FileName,
                        TypSouboru = f.FileType.ToString(),
                        VelikostSouboru = f.FileSize,
                        DatumNahrani = f.UploadedAt
                    }).ToList()
                }).ToList(),
                Events = pojistenec.InsuranceClaims.Where(u => u.InsuranceContract == null).Select(u => new EventData
                {
                    Id = u.Id,
                    CisloUdalosti = u.ClaimNumber,
                    DatumNahlaseni = u.ReportedAt,
                    DatumUdalosti = u.IncidentDateTime,
                    Popis = u.DamageDescription,
                    VyseSkody = u.EstimatedDamage,
                    VyseNahrady = u.PaymentAmount,
                    Status = u.Status.ToString(),
                    Files = u.AttachedFiles.Select(f => new FileData
                    {
                        Id = f.Id,
                        NazevSouboru = f.FileName,
                        TypSouboru = f.FileType.ToString(),
                        VelikostSouboru = f.FileSize,
                        DatumNahrani = f.UploadedAt
                    }).ToList()
                }).ToList()
            };

            return export;
        }

        /// <summary>
        /// Anonymizuje osobní data pojištěnce
        /// </summary>
        public async Task<bool> AnonymizePersonalDataAsync(int pojistenecId, string reason, int anonymizedByUserId)
        {
            if (!await CanAnonymizeAsync(pojistenecId))
                return false;

            await LogDataAccessAsync(pojistenecId, anonymizedByUserId, GdprAction.DataAnonymization, $"Anonymizace dat: {reason}");

            var pojistenec = await _context.InsuredPersons.FindAsync(pojistenecId);
            if (pojistenec == null)
                return false;

            // Anonymizace osobních údajů
            pojistenec.FirstName = "ANONYMIZOVÁNO";
            pojistenec.LastName = "ANONYMIZOVÁNO";
            pojistenec.Email = $"anonymized_{pojistenecId}@example.com";
            pojistenec.Phone = "ANONYMIZOVÁNO";
            pojistenec.Address = "ANONYMIZOVÁNO";
            pojistenec.NationalId = null;
            pojistenec.DateOfBirth = new DateTime(1900, 1, 1);

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Anonymizace dokončena pro pojištěnce {pojistenecId} uživatelem {anonymizedByUserId}");
            return true;
        }

        /// <summary>
        /// Kontroluje, zda lze pojištěnce anonymizovat
        /// </summary>
        public async Task<bool> CanAnonymizeAsync(int pojistenecId)
        {
            // Nelze anonymizovat pokud má aktivní smlouvy nebo nevyřešené události
            var hasActiveContracts = await _context.InsuranceContracts
                .AnyAsync(s => s.InsuredPersonId == pojistenecId && 
                              s.Status == ContractStatus.Active);

            var hasUnresolvedEvents = await _context.InsuranceClaims
                .Include(u => u.InsuranceContract)
                .AnyAsync(u => u.InsuranceContract.InsuredPersonId == pojistenecId && 
                              u.Status != ClaimStatus.Resolved && 
                              u.Status != ClaimStatus.Rejected);

            return !hasActiveContracts && !hasUnresolvedEvents;
        }

        /// <summary>
        /// Zaznamenává přístup k osobním datům (implementace rozhraní)
        /// </summary>
        public async Task LogDataAccessAsync(int pojistenecId, int userId, GdprAction action, string details = "")
        {
            await LogDataAccessAsync(pojistenecId, userId, action, details, null, null);
        }

        /// <summary>
        /// Zaznamenává přístup k osobním datům (rozšířená verze)
        /// </summary>
        public async Task LogDataAccessAsync(int pojistenecId, int uzivatelId, GdprAction action, string details, string? ipAddress = null, string? userAgent = null)
        {
            var auditLog = new GdprAuditLog
            {
                PojistenecId = pojistenecId,
                UzivatelId = uzivatelId,
                Akce = action,
                Detaily = details,
                IpAdresa = ipAddress,
                UserAgent = userAgent,
                DatumCas = DateTime.UtcNow
            };

            _context.GdprAuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Získává audit log pro pojištěnce (implementace rozhraní)
        /// </summary>
        public async Task<List<GdprAuditLog>> GetAuditLogAsync(int pojistenecId)
        {
            return await GetAuditLogAsync(pojistenecId, null, null);
        }

        /// <summary>
        /// Získává audit log pro pojištěnce (rozšířená verze)
        /// </summary>
        public async Task<List<GdprAuditLog>> GetAuditLogAsync(int pojistenecId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.GdprAuditLogs
                .Include(a => a.Uzivatel)
                .Where(a => a.PojistenecId == pojistenecId);

            if (fromDate.HasValue)
                query = query.Where(a => a.DatumCas >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a => a.DatumCas <= toDate.Value);

            return await query
                .OrderByDescending(a => a.DatumCas)
                .ToListAsync();
        }

        /// <summary>
        /// Kontroluje platnost souhlasu
        /// </summary>
        public async Task<bool> HasValidConsentAsync(int pojistenecId, PersonalDataCategory category)
        {
            return await _context.GdprConsents
                .AnyAsync(c => c.PojistenecId == pojistenecId &&
                              c.Kategorie == category &&
                              c.JeAktivni &&
                              c.DatumOdvolani == null);
        }

        /// <summary>
        /// Zaznamenává souhlas (implementace rozhraní)
        /// </summary>
        public async Task RecordConsentAsync(int pojistenecId, PersonalDataCategory category, string purpose, int consentGivenBy)
        {
            await RecordConsentAsync(pojistenecId, category, purpose, consentGivenBy, null);
        }

        /// <summary>
        /// Zaznamenává souhlas (rozšířená verze)
        /// </summary>
        public async Task<bool> RecordConsentAsync(int pojistenecId, PersonalDataCategory category, string purpose, int recordedByUserId, string? ipAddress = null)
        {
            // Zkontroluj, zda už neexistuje aktivní souhlas
            var existingConsent = await _context.GdprConsents
                .FirstOrDefaultAsync(c => c.PojistenecId == pojistenecId &&
                                         c.Kategorie == category &&
                                         c.JeAktivni);

            if (existingConsent != null)
                return false; // Souhlas už existuje

            var consent = new GdprConsent
            {
                PojistenecId = pojistenecId,
                Kategorie = category,
                Ucel = purpose,
                UdelenoKym = recordedByUserId,
                IpAdresa = ipAddress,
                DatumUdeleni = DateTime.UtcNow,
                JeAktivni = true
            };

            _context.GdprConsents.Add(consent);
            await _context.SaveChangesAsync();

            await LogDataAccessAsync(pojistenecId, recordedByUserId, GdprAction.ConsentGranted, 
                $"Udělen souhlas pro kategorii {category}: {purpose}");

            return true;
        }

        /// <summary>
        /// Odvolává souhlas (implementace rozhraní)
        /// </summary>
        public async Task RevokeConsentAsync(int pojistenecId, PersonalDataCategory category, int revokedBy)
        {
            await RevokeConsentAsync(pojistenecId, category, revokedBy, "Odvolání souhlasu");
        }

        /// <summary>
        /// Odvolává souhlas (rozšířená verze)
        /// </summary>
        public async Task<bool> RevokeConsentAsync(int pojistenecId, PersonalDataCategory category, int revokedByUserId, string reason)
        {
            var consent = await _context.GdprConsents
                .FirstOrDefaultAsync(c => c.PojistenecId == pojistenecId &&
                                         c.Kategorie == category &&
                                         c.JeAktivni);

            if (consent == null)
                return false;

            consent.JeAktivni = false;
            consent.DatumOdvolani = DateTime.UtcNow;
            consent.OdvolanoKym = revokedByUserId;
            consent.DuvodOdvolani = reason;

            await _context.SaveChangesAsync();

            await LogDataAccessAsync(pojistenecId, revokedByUserId, GdprAction.ConsentRevoked,
                $"Odvolán souhlas pro kategorii {category}: {reason}");

            return true;
        }

        /// <summary>
        /// Extrahuje osobní data z entity pomocí atributů
        /// </summary>
        private PersonalData ExtractPersonalData(object entity)
        {
            var personalData = new PersonalData();
            var properties = entity.GetType().GetProperties();

            foreach (var property in properties)
            {
                var personalDataAttr = property.GetCustomAttribute<PersonalDataAttribute>();
                if (personalDataAttr != null)
                {
                    var value = property.GetValue(entity);
                    if (value != null)
                    {
                        var encryptedAttr = property.GetCustomAttribute<EncryptedAttribute>();
                        var displayValue = encryptedAttr != null ? 
                            _encryptionService.Decrypt(value.ToString()!) : 
                            value.ToString()!;

                        personalData.Data[property.Name] = new Dictionary<string, object>
                        {
                            ["value"] = displayValue,
                            ["category"] = personalDataAttr.Category.ToString(),
                            ["purpose"] = personalDataAttr.Purpose,
                            ["required"] = personalDataAttr.IsRequired
                        };
                    }
                }
            }

            return personalData;
        }
    }
}