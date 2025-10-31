# Insurance System API

Kompletní REST API pro správu pojištěnců, pojistných smluv a pojistných událostí, včetně správy souborů a GDPR operací. Swagger UI je dostupné na `http://localhost:5000/api` a obsahuje popisy jednotlivých endpointů.

## Funkce API
- Autentizace pomocí JWT (přihlášení, profil, změna hesla, ověření tokenu)
- Správa uživatelů (role, aktivace/deaktivace, reset hesla)
- Správa pojištěnců (CRUD, vyhledávání, stránkování)
- Správa pojistných smluv (CRUD, statistiky, související události)
- Správa pojistných událostí (CRUD, workflow stavu, vyúčtování)
- Správa souborů k smlouvám a událostem (nahrání, seznam, stažení)
- GDPR operace (export dat, anonymizace, souhlasy, audit log)

## Technologie a požadavky
- `.NET SDK` `9.0`
- `ASP.NET Core` `9.0`
- `Entity Framework Core` s `Pomelo.EntityFrameworkCore.MySql`
- Databáze: `MySQL 8.x` (nebo kompatibilní MariaDB)
- Volitelně: `dotnet-ef` pro ruční práci s migracemi

## Lokální spuštění
1. Nainstalujte potřebný software:
   - `dotnet --info` musí ukazovat `.NET SDK 9.0`
   - MySQL server běžící lokálně (např. `localhost:3306`)
2. Upravte `appsettings.json` (viz Konfigurace níže).
3. Obnovte a sestavte projekt:
   - `dotnet restore`
   - `dotnet build -c Debug`
4. Spusťte API:
   - `dotnet run`
   - API poslouchá na `http://localhost:5000` (Swagger UI na `http://localhost:5000/api`).

Poznámka: Databázové migrace se aplikují automaticky při startu (`context.Database.Migrate()`), pokud je platná connection string.

## Konfigurace (`appsettings.json`)
Příklad nastavení pro vývoj:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=InsuranceSystemDB;User=root;Password=yourpassword;"
  },
  "Jwt": {
    "Key": "replace-with-long-random-secret-key",
    "Issuer": "InsuranceSystemAPI",
    "Audience": "InsuranceSystemAPI.Clients"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

- `DefaultConnection`: MySQL připojení. Uživatel musí mít oprávnění `CREATE DATABASE/TABLE`.
- `Jwt:Key`: silný tajný klíč (min. 32+ znaků).
- `Jwt:Issuer`, `Jwt:Audience`: libovolné konzistentní identifikátory.

## Swagger a testování
- Swagger UI: `http://localhost:5000/api`
- OpenAPI JSON: `http://localhost:5000/swagger/v1/swagger.json`
- Klikněte na tlačítko „Authorize“ a zadejte `Bearer <token>` pro autorizované operace.

## Autentizace
- `POST /api/Auth/login`
  - Request body:
    ```json
    { "username": "admin", "password": "password123" }
    ```
  - Response (`LoginResponseDto`):
    ```json
    {
      "token": "<JWT>",
      "expiration": "2025-11-01T12:34:56Z",
      "user": {
        "id": 1,
        "username": "admin",
        "firstName": "Admin",
        "lastName": "User",
        "role": "Admin",
        "isActive": true
      }
    }
    ```
- `POST /api/Auth/register` (pouze `Admin`)
- `GET /api/Auth/profile` (vyžaduje JWT)
- `POST /api/Auth/change-password` (vyžaduje JWT)
- `GET /api/Auth/verify` (ověření platnosti tokenu)

## Přehled endpointů

### Users (pouze Admin)
- `GET /api/Users` – seznam s filtrováním a stránkováním (`UserSearchDto`)
- `GET /api/Users/{id}` – detail
- `POST /api/Users` – vytvoření (přes `RegisterRequestDto`)
- `PUT /api/Users/{id}` – aktualizace (`UpdateUserDto`)
- `DELETE /api/Users/{id}` – smazání (blokace při navázaných záznamech)
- `PUT /api/Users/{id}/activation` – přepnutí aktivace
- `PUT /api/Users/{id}/reset-password` – vygeneruje nové heslo

### InsuredPersons (autorizace vyžadována)
- `GET /api/InsuredPersons` – seznam, query `InsuredPersonSearchDto` (`FirstName`, `LastName`, `Email`, `PersonalNumber`, `IsActive`, `Page`, `PageSize`)
- `GET /api/InsuredPersons/{id}` – detail
- `POST /api/InsuredPersons` – vytvoření (`CreateInsuredPersonDto`)
- `PUT /api/InsuredPersons/{id}` – aktualizace (`UpdateInsuredPersonDto`)

### Pojistné smlouvy (PojistneSmlouvy)
- `GET /api/PojistneSmlouvy` – seznam s filtrováním (`InsuranceContractSearchDto`)
- `PUT /api/PojistneSmlouvy/{id}` – aktualizace (`UpdateInsuranceContractDto`)
- `GET /api/PojistneSmlouvy/{id}/claims` – související pojistné události

`InsuranceContractDto` obsahuje mimo jiné:
- `ContractNumber`, `InsuranceType`, `InsuranceTypeText`, `InsuredAmount`, `InsuranceLimit`,
- `Status`, `StatusText`, `IsPaid`, `ValidFrom`, `ValidTo`, `AnnualPremium`, `Notes`, `CreatedAt`,
- `InsuredPersonId`, `InsuredPersonName`, `ManagerId`, `ManagerName`,
- `IsValid`, `DaysToExpiry`, `ClaimCount`.

### Pojistné události (InsuranceClaims)
- `GET /api/InsuranceClaims` – seznam s filtrováním (`InsuranceClaimSearchDto`)
- `GET /api/InsuranceClaims/{id}` – detail (`InsuranceClaimDto`)
- `PUT /api/InsuranceClaims/{id}` – aktualizace (`UpdateInsuranceClaimDto`)
- `POST /api/InsuranceClaims/{id}/process` – zpracování/uzavření události (pokud controller poskytuje)

`InsuranceClaimDto` obsahuje mimo jiné:
- `IncidentDateTime`, `DamageDescription`, `IncidentLocation`, `Witnesses`,
- `EstimatedDamage`, `MonetaryReserve`, `PaymentAmount`, `InsuranceCompanyNumber`,
- `ClaimStatus`, `StatusText`, `AdjusterNotes`, `ReportedAt`, `ResolvedAt`,
- `InsuranceContractId`, `ContractNumber`, `InsuranceContractNumber`, `InsuredPersonName`,
- `AdjusterId`, `AdjusterName`, `ReporterId`, `ReporterName`,
- `DaysSinceReported`, `IsResolved`, `FileCount`.

### Soubory (Files)
- `POST /api/Files/contract/{contractId}` – nahrání souboru ke smlouvě (form-data `FileUploadDto`)
- `POST /api/Files/claim/{claimId}` – nahrání souboru k události (form-data `FileUploadDto`)
- `GET /api/Files/contract/{contractId}` – seznam souborů ke smlouvě
- `GET /api/Files/claim/{claimId}` – seznam souborů k události
- `GET /api/Files/contract/{contractId}/download/{fileId}` – stažení souboru ke smlouvě
- `GET /api/Files/claim/{claimId}/download/{fileId}` – stažení souboru k události

### GDPR
- `GET /api/Gdpr/export/{insuredPersonId}` – export všech osobních dat pojištěnce
- `POST /api/Gdpr/anonymize/{insuredPersonId}` – anonymizace osobních dat (Admin)
- `GET /api/Gdpr/can-anonymize/{insuredPersonId}` – ověření možnosti anonymizace (Admin)
- `GET /api/Gdpr/audit-log/{insuredPersonId}` – audit log pojištěnce (Admin)
- `POST /api/Gdpr/consent/{insuredPersonId}` – zaznamenání souhlasu (Admin/Makler)
- `DELETE /api/Gdpr/consent/{insuredPersonId}` – odvolání souhlasu (Admin/Makler)
- `GET /api/Gdpr/consent/{insuredPersonId}/check` – kontrola platnosti souhlasu (Admin/Makler)

## Paginace a odpovědi
Seznamové endpointy vracejí strukturu `PagedResult<T>`:
```json
{
  "items": [ /* položky */ ],
  "totalCount": 123,
  "page": 1,
  "pageSize": 20,
  "totalPages": 7,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

## Role a autorizace
- `Admin`: plný přístup včetně správy uživatelů a GDPR.
- `Broker` (`Makler`): správa pojištěnců, smluv, nahrávání souborů.
- `Adjuster`: zpracování pojistných událostí, nahrávání souborů k událostem.