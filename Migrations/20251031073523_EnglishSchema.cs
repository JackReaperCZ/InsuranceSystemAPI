using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceSystemAPI.Migrations
{
    /// <inheritdoc />
    public partial class EnglishSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gdpr_audit_logs_Pojistenci_pojistenec_id",
                table: "gdpr_audit_logs");

            migrationBuilder.DropForeignKey(
                name: "FK_gdpr_audit_logs_Uzivatele_uzivatel_id",
                table: "gdpr_audit_logs");

            migrationBuilder.DropForeignKey(
                name: "FK_gdpr_consents_Pojistenci_pojistenec_id",
                table: "gdpr_consents");

            migrationBuilder.DropForeignKey(
                name: "FK_gdpr_consents_Uzivatele_odvolano_kym",
                table: "gdpr_consents");

            migrationBuilder.DropForeignKey(
                name: "FK_gdpr_consents_Uzivatele_udeljen_kym",
                table: "gdpr_consents");

            migrationBuilder.DropTable(
                name: "SouborySmluv");

            migrationBuilder.DropTable(
                name: "SouboryUdalosti");

            migrationBuilder.DropTable(
                name: "PojistneUdalosti");

            migrationBuilder.DropTable(
                name: "PojistneSmlouvy");

            migrationBuilder.DropTable(
                name: "Pojistenci");

            migrationBuilder.DropTable(
                name: "Uzivatele");

            migrationBuilder.CreateTable(
                name: "InsuredPersons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateOfBirth = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NationalId = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdCardNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuredPersons", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HashedPassword = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FirstName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Role = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InsuranceContracts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ContractNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InsuranceType = table.Column<int>(type: "int", nullable: false),
                    InsuredAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InsuranceLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsPaid = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AnnualPremium = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Notes = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    InsuredPersonId = table.Column<int>(type: "int", nullable: false),
                    ManagerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceContracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuranceContracts_InsuredPersons_InsuredPersonId",
                        column: x => x.InsuredPersonId,
                        principalTable: "InsuredPersons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InsuranceContracts_Users_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ContractFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FileName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FilePath = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UploadedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    InsuranceContractId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractFiles_InsuranceContracts_InsuranceContractId",
                        column: x => x.InsuranceContractId,
                        principalTable: "InsuranceContracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InsuranceClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClaimNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IncidentDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DamageDescription = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IncidentLocation = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Witnesses = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EstimatedDamage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MonetaryReserve = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PaymentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DamageAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CompensationAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InsuranceCompanyNumber = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AdjusterNotes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReportedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    InsuranceContractId = table.Column<int>(type: "int", nullable: false),
                    AdjusterId = table.Column<int>(type: "int", nullable: true),
                    ReporterId = table.Column<int>(type: "int", nullable: true),
                    InsuredPersonId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuranceClaims_InsuranceContracts_InsuranceContractId",
                        column: x => x.InsuranceContractId,
                        principalTable: "InsuranceContracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InsuranceClaims_InsuredPersons_InsuredPersonId",
                        column: x => x.InsuredPersonId,
                        principalTable: "InsuredPersons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InsuranceClaims_Users_AdjusterId",
                        column: x => x.AdjusterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_InsuranceClaims_Users_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ClaimFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FileName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FilePath = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    FileCategory = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UploadedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    InsuranceClaimId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClaimFiles_InsuranceClaims_InsuranceClaimId",
                        column: x => x.InsuranceClaimId,
                        principalTable: "InsuranceClaims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "InsuredPersons",
                columns: new[] { "Id", "Address", "CreatedAt", "DateOfBirth", "Email", "FirstName", "IdCardNumber", "IsActive", "LastName", "NationalId", "Phone", "UpdatedAt" },
                values: new object[] { 1, "Hlavní 123, Praha 1", new DateTime(2025, 10, 31, 7, 35, 22, 468, DateTimeKind.Utc).AddTicks(294), new DateTime(1980, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "jan.novak@email.cz", "Jan", null, true, "Novák", "8005150123", "+420123456789", null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "HashedPassword", "IsActive", "LastLogin", "LastName", "Phone", "Role", "Username" },
                values: new object[] { 1, new DateTime(2025, 10, 31, 7, 35, 22, 466, DateTimeKind.Utc).AddTicks(8929), "admin@insurance.cz", "Administrátor", "$2a$11$m/oUCvFhKx/pMbQ1BtLYCecIdV8Jq6NHOXnl7J2U0wV.Q5ljVbepW", true, null, "Systému", null, 1, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_ClaimFiles_InsuranceClaimId",
                table: "ClaimFiles",
                column: "InsuranceClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractFiles_InsuranceContractId",
                table: "ContractFiles",
                column: "InsuranceContractId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceClaims_AdjusterId",
                table: "InsuranceClaims",
                column: "AdjusterId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceClaims_InsuranceContractId",
                table: "InsuranceClaims",
                column: "InsuranceContractId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceClaims_InsuredPersonId",
                table: "InsuranceClaims",
                column: "InsuredPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceClaims_ReporterId",
                table: "InsuranceClaims",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceContracts_ContractNumber",
                table: "InsuranceContracts",
                column: "ContractNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceContracts_InsuredPersonId",
                table: "InsuranceContracts",
                column: "InsuredPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceContracts_ManagerId",
                table: "InsuranceContracts",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuredPersons_IdCardNumber",
                table: "InsuredPersons",
                column: "IdCardNumber",
                unique: true,
                filter: "[IdCardNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InsuredPersons_NationalId",
                table: "InsuredPersons",
                column: "NationalId",
                unique: true,
                filter: "[NationalId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_gdpr_audit_logs_InsuredPersons_pojistenec_id",
                table: "gdpr_audit_logs",
                column: "pojistenec_id",
                principalTable: "InsuredPersons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gdpr_audit_logs_Users_uzivatel_id",
                table: "gdpr_audit_logs",
                column: "uzivatel_id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_gdpr_consents_InsuredPersons_pojistenec_id",
                table: "gdpr_consents",
                column: "pojistenec_id",
                principalTable: "InsuredPersons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gdpr_consents_Users_odvolano_kym",
                table: "gdpr_consents",
                column: "odvolano_kym",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_gdpr_consents_Users_udeljen_kym",
                table: "gdpr_consents",
                column: "udeljen_kym",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gdpr_audit_logs_InsuredPersons_pojistenec_id",
                table: "gdpr_audit_logs");

            migrationBuilder.DropForeignKey(
                name: "FK_gdpr_audit_logs_Users_uzivatel_id",
                table: "gdpr_audit_logs");

            migrationBuilder.DropForeignKey(
                name: "FK_gdpr_consents_InsuredPersons_pojistenec_id",
                table: "gdpr_consents");

            migrationBuilder.DropForeignKey(
                name: "FK_gdpr_consents_Users_odvolano_kym",
                table: "gdpr_consents");

            migrationBuilder.DropForeignKey(
                name: "FK_gdpr_consents_Users_udeljen_kym",
                table: "gdpr_consents");

            migrationBuilder.DropTable(
                name: "ClaimFiles");

            migrationBuilder.DropTable(
                name: "ContractFiles");

            migrationBuilder.DropTable(
                name: "InsuranceClaims");

            migrationBuilder.DropTable(
                name: "InsuranceContracts");

            migrationBuilder.DropTable(
                name: "InsuredPersons");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.CreateTable(
                name: "Pojistenci",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Adresa = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CisloOP = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatumNarozeni = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DatumUpravy = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DatumVytvoreni = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    JeAktivni = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Jmeno = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Prijmeni = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RodneCislo = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefon = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pojistenci", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Uzivatele",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DatumVytvoreni = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HashedPassword = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    JeAktivni = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Jmeno = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PosledniPrihlaseni = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Prijmeni = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Telefon = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UzivatelskeJmeno = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uzivatele", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PojistneSmlouvy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PojistenecId = table.Column<int>(type: "int", nullable: false),
                    SpravceId = table.Column<int>(type: "int", nullable: true),
                    CisloSmlouvy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatumUpravy = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DatumVytvoreni = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    JeZaplacena = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LimitPojisteni = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PlatnostDo = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PlatnostOd = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PojistnaPartka = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Poznamky = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RocniPojistne = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TypPojisteni = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PojistneSmlouvy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PojistneSmlouvy_Pojistenci_PojistenecId",
                        column: x => x.PojistenecId,
                        principalTable: "Pojistenci",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PojistneSmlouvy_Uzivatele_SpravceId",
                        column: x => x.SpravceId,
                        principalTable: "Uzivatele",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PojistneUdalosti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NahlasovatelId = table.Column<int>(type: "int", nullable: true),
                    PojistnaSmlouvaId = table.Column<int>(type: "int", nullable: false),
                    VyrizovatelId = table.Column<int>(type: "int", nullable: true),
                    CastkaPlneni = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CisloPojistovny = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CisloUdalosti = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatumCasUdalosti = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DatumNahlaseni = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DatumUpravy = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DatumVyrizeni = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    MistoUdalosti = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OdhadSkody = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PenezniRezerva = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PojistenecId = table.Column<int>(type: "int", nullable: true),
                    PopisSkody = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PoznamkyLikvidatora = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Svedci = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VyseNahrady = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    VyseSkody = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PojistneUdalosti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PojistneUdalosti_Pojistenci_PojistenecId",
                        column: x => x.PojistenecId,
                        principalTable: "Pojistenci",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PojistneUdalosti_PojistneSmlouvy_PojistnaSmlouvaId",
                        column: x => x.PojistnaSmlouvaId,
                        principalTable: "PojistneSmlouvy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PojistneUdalosti_Uzivatele_NahlasovatelId",
                        column: x => x.NahlasovatelId,
                        principalTable: "Uzivatele",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PojistneUdalosti_Uzivatele_VyrizovatelId",
                        column: x => x.VyrizovatelId,
                        principalTable: "Uzivatele",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SouborySmluv",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PojistnaSmlouvaId = table.Column<int>(type: "int", nullable: false),
                    CestaSouboru = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatumNahrani = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    NazevSouboru = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Popis = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TypSouboru = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VelikostSouboru = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SouborySmluv", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SouborySmluv_PojistneSmlouvy_PojistnaSmlouvaId",
                        column: x => x.PojistnaSmlouvaId,
                        principalTable: "PojistneSmlouvy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SouboryUdalosti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PojistnaUdalostId = table.Column<int>(type: "int", nullable: false),
                    CestaSouboru = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatumNahrani = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    KategorieSouboru = table.Column<int>(type: "int", nullable: false),
                    NazevSouboru = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Popis = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TypSouboru = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VelikostSouboru = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SouboryUdalosti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SouboryUdalosti_PojistneUdalosti_PojistnaUdalostId",
                        column: x => x.PojistnaUdalostId,
                        principalTable: "PojistneUdalosti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Pojistenci",
                columns: new[] { "Id", "Adresa", "CisloOP", "DatumNarozeni", "DatumUpravy", "DatumVytvoreni", "Email", "JeAktivni", "Jmeno", "Prijmeni", "RodneCislo", "Telefon" },
                values: new object[] { 1, "Hlavní 123, Praha 1", null, new DateTime(1980, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2025, 10, 30, 12, 56, 46, 136, DateTimeKind.Utc).AddTicks(5818), "jan.novak@email.cz", true, "Jan", "Novák", "8005150123", "+420123456789" });

            migrationBuilder.InsertData(
                table: "Uzivatele",
                columns: new[] { "Id", "DatumVytvoreni", "Email", "HashedPassword", "JeAktivni", "Jmeno", "PosledniPrihlaseni", "Prijmeni", "Role", "Telefon", "UzivatelskeJmeno" },
                values: new object[] { 1, new DateTime(2025, 10, 30, 12, 56, 46, 135, DateTimeKind.Utc).AddTicks(7557), "admin@insurance.cz", "$2a$11$dKfLMxmjGLNvB7aMiCcOCOLG3B.3ESc9LHsHPqERFej6bHlHr2Wru", true, "Administrátor", null, "Systému", 1, null, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Pojistenci_CisloOP",
                table: "Pojistenci",
                column: "CisloOP",
                unique: true,
                filter: "[CisloOP] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Pojistenci_RodneCislo",
                table: "Pojistenci",
                column: "RodneCislo",
                unique: true,
                filter: "[RodneCislo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PojistneSmlouvy_CisloSmlouvy",
                table: "PojistneSmlouvy",
                column: "CisloSmlouvy",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PojistneSmlouvy_PojistenecId",
                table: "PojistneSmlouvy",
                column: "PojistenecId");

            migrationBuilder.CreateIndex(
                name: "IX_PojistneSmlouvy_SpravceId",
                table: "PojistneSmlouvy",
                column: "SpravceId");

            migrationBuilder.CreateIndex(
                name: "IX_PojistneUdalosti_NahlasovatelId",
                table: "PojistneUdalosti",
                column: "NahlasovatelId");

            migrationBuilder.CreateIndex(
                name: "IX_PojistneUdalosti_PojistenecId",
                table: "PojistneUdalosti",
                column: "PojistenecId");

            migrationBuilder.CreateIndex(
                name: "IX_PojistneUdalosti_PojistnaSmlouvaId",
                table: "PojistneUdalosti",
                column: "PojistnaSmlouvaId");

            migrationBuilder.CreateIndex(
                name: "IX_PojistneUdalosti_VyrizovatelId",
                table: "PojistneUdalosti",
                column: "VyrizovatelId");

            migrationBuilder.CreateIndex(
                name: "IX_SouborySmluv_PojistnaSmlouvaId",
                table: "SouborySmluv",
                column: "PojistnaSmlouvaId");

            migrationBuilder.CreateIndex(
                name: "IX_SouboryUdalosti_PojistnaUdalostId",
                table: "SouboryUdalosti",
                column: "PojistnaUdalostId");

            migrationBuilder.CreateIndex(
                name: "IX_Uzivatele_Email",
                table: "Uzivatele",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Uzivatele_UzivatelskeJmeno",
                table: "Uzivatele",
                column: "UzivatelskeJmeno",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_gdpr_audit_logs_Pojistenci_pojistenec_id",
                table: "gdpr_audit_logs",
                column: "pojistenec_id",
                principalTable: "Pojistenci",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gdpr_audit_logs_Uzivatele_uzivatel_id",
                table: "gdpr_audit_logs",
                column: "uzivatel_id",
                principalTable: "Uzivatele",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_gdpr_consents_Pojistenci_pojistenec_id",
                table: "gdpr_consents",
                column: "pojistenec_id",
                principalTable: "Pojistenci",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gdpr_consents_Uzivatele_odvolano_kym",
                table: "gdpr_consents",
                column: "odvolano_kym",
                principalTable: "Uzivatele",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_gdpr_consents_Uzivatele_udeljen_kym",
                table: "gdpr_consents",
                column: "udeljen_kym",
                principalTable: "Uzivatele",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
