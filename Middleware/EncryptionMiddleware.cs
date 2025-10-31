using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection;
using InsuranceSystemAPI.Services;

namespace InsuranceSystemAPI.Middleware
{
    /// <summary>
    /// Middleware pro automatické šifrování a dešifrování dat při ukládání/načítání z databáze
    /// </summary>
    public class EncryptionInterceptor : SaveChangesInterceptor
    {
        private readonly IEncryptionService _encryptionService;

        public EncryptionInterceptor(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            EncryptData(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            EncryptData(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        /// <summary>
        /// Šifruje data před uložením do databáze
        /// </summary>
        private void EncryptData(DbContext? context)
        {
            if (context == null) return;

            var entries = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                EncryptEntityProperties(entry);
            }
        }

        /// <summary>
        /// Šifruje vlastnosti entity označené atributem [Encrypted]
        /// </summary>
        private void EncryptEntityProperties(EntityEntry entry)
        {
            var entityType = entry.Entity.GetType();
            var properties = entityType.GetProperties()
                .Where(p => p.GetCustomAttribute<EncryptedAttribute>() != null);

            foreach (var property in properties)
            {
                var value = property.GetValue(entry.Entity);
                if (value != null && value is string stringValue && !string.IsNullOrEmpty(stringValue))
                {
                    // Kontrola, zda už není šifrováno (jednoduchá kontrola délky a formátu)
                    if (!IsAlreadyEncrypted(stringValue))
                    {
                        var encryptedValue = _encryptionService.Encrypt(stringValue);
                        property.SetValue(entry.Entity, encryptedValue);

                        // Vytvoření hash pro vyhledávání, pokud je požadováno
                        var encryptedAttr = property.GetCustomAttribute<EncryptedAttribute>();
                        if (encryptedAttr?.CreateSearchHash == true)
                        {
                            var hashPropertyName = $"{property.Name}Hash";
                            var hashProperty = entityType.GetProperty(hashPropertyName);
                            if (hashProperty != null)
                            {
                                var hash = _encryptionService.CreateHash(stringValue);
                                hashProperty.SetValue(entry.Entity, hash);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Jednoduchá kontrola, zda je hodnota už šifrovaná
        /// </summary>
        private bool IsAlreadyEncrypted(string value)
        {
            // Šifrované hodnoty jsou obvykle delší a obsahují speciální znaky
            // Toto je zjednodušená kontrola - v produkci by měla být robustnější
            return value.Length > 100 && (value.Contains("+") || value.Contains("/") || value.Contains("="));
        }
    }

    /// <summary>
    /// Extension metody pro registraci encryption interceptoru
    /// </summary>
    public static class EncryptionExtensions
    {
        /// <summary>
        /// Přidá encryption interceptor do DbContext
        /// </summary>
        public static DbContextOptionsBuilder AddEncryptionInterceptor(
            this DbContextOptionsBuilder optionsBuilder, 
            IServiceProvider serviceProvider)
        {
            var encryptionService = serviceProvider.GetRequiredService<IEncryptionService>();
            optionsBuilder.AddInterceptors(new EncryptionInterceptor(encryptionService));
            return optionsBuilder;
        }
    }

    /// <summary>
    /// Service pro dešifrování dat při čtení
    /// </summary>
    public class DecryptionService
    {
        private readonly IEncryptionService _encryptionService;

        public DecryptionService(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }

        /// <summary>
        /// Dešifruje entity po načtení z databáze
        /// </summary>
        public void DecryptEntity<T>(T entity) where T : class
        {
            if (entity == null) return;

            var entityType = typeof(T);
            var properties = entityType.GetProperties()
                .Where(p => p.GetCustomAttribute<EncryptedAttribute>() != null);

            foreach (var property in properties)
            {
                var value = property.GetValue(entity);
                if (value != null && value is string stringValue && !string.IsNullOrEmpty(stringValue))
                {
                    try
                    {
                        var decryptedValue = _encryptionService.Decrypt(stringValue);
                        property.SetValue(entity, decryptedValue);
                    }
                    catch
                    {
                        // Pokud dešifrování selže, ponecháme původní hodnotu
                        // V produkci by se mělo logovat
                    }
                }
            }
        }

        /// <summary>
        /// Dešifruje kolekci entit
        /// </summary>
        public void DecryptEntities<T>(IEnumerable<T> entities) where T : class
        {
            foreach (var entity in entities)
            {
                DecryptEntity(entity);
            }
        }
    }
}