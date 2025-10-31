using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceSystemAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Pojistenci",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Jmeno = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Prijmeni = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatumNarozeni = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Telefon = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Adresa = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RodneCislo = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CisloOP = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    JeAktivni = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DatumVytvoreni = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DatumUpravy = table.Column<DateTime>(type: "datetime(6)", nullable: true)
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
                    UzivatelskeJmeno = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HashedPassword = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Jmeno = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Prijmeni = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefon = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Role = table.Column<int>(type: "int", nullable: false),
                    JeAktivni = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DatumVytvoreni = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PosledniPrihlaseni = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uzivatele", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "gdpr_audit_logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    uzivatel_id = table.Column<int>(type: "int", nullable: false),
                    pojistenec_id = table.Column<int>(type: "int", nullable: false),
                    akce = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    detaily = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ip_adresa = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_agent = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    datum_cas = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gdpr_audit_logs", x => x.id);
                    table.ForeignKey(
                        name: "FK_gdpr_audit_logs_Pojistenci_pojistenec_id",
                        column: x => x.pojistenec_id,
                        principalTable: "Pojistenci",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gdpr_audit_logs_Uzivatele_uzivatel_id",
                        column: x => x.uzivatel_id,
                        principalTable: "Uzivatele",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "gdpr_consents",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    pojistenec_id = table.Column<int>(type: "int", nullable: false),
                    kategorie = table.Column<int>(type: "int", nullable: false),
                    ucel = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    datum_udeleni = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    udeljen_kym = table.Column<int>(type: "int", nullable: false),
                    datum_odvolani = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    odvolano_kym = table.Column<int>(type: "int", nullable: true),
                    duvod_odvolani = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    je_aktivni = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    verze_podminek = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ip_adresa = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gdpr_consents", x => x.id);
                    table.ForeignKey(
                        name: "FK_gdpr_consents_Pojistenci_pojistenec_id",
                        column: x => x.pojistenec_id,
                        principalTable: "Pojistenci",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gdpr_consents_Uzivatele_odvolano_kym",
                        column: x => x.odvolano_kym,
                        principalTable: "Uzivatele",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gdpr_consents_Uzivatele_udeljen_kym",
                        column: x => x.udeljen_kym,
                        principalTable: "Uzivatele",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PojistneSmlouvy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CisloSmlouvy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TypPojisteni = table.Column<int>(type: "int", nullable: false),
                    PojistnaPartka = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LimitPojisteni = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    JeZaplacena = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PlatnostOd = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PlatnostDo = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RocniPojistne = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Poznamky = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatumVytvoreni = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DatumUpravy = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    PojistenecId = table.Column<int>(type: "int", nullable: false),
                    SpravceId = table.Column<int>(type: "int", nullable: true)
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
                    CisloUdalosti = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatumCasUdalosti = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PopisSkody = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MistoUdalosti = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Svedci = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OdhadSkody = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PenezniRezerva = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CastkaPlneni = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    VyseSkody = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    VyseNahrady = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CisloPojistovny = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PoznamkyLikvidatora = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatumNahlaseni = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DatumVyrizeni = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DatumUpravy = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    PojistnaSmlouvaId = table.Column<int>(type: "int", nullable: false),
                    VyrizovatelId = table.Column<int>(type: "int", nullable: true),
                    NahlasovatelId = table.Column<int>(type: "int", nullable: true),
                    PojistenecId = table.Column<int>(type: "int", nullable: true)
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
                    NazevSouboru = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CestaSouboru = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TypSouboru = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VelikostSouboru = table.Column<long>(type: "bigint", nullable: false),
                    Popis = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatumNahrani = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PojistnaSmlouvaId = table.Column<int>(type: "int", nullable: false)
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
                    NazevSouboru = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CestaSouboru = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TypSouboru = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VelikostSouboru = table.Column<long>(type: "bigint", nullable: false),
                    KategorieSouboru = table.Column<int>(type: "int", nullable: false),
                    Popis = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatumNahrani = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PojistnaUdalostId = table.Column<int>(type: "int", nullable: false)
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
                name: "IX_gdpr_audit_logs_pojistenec_id_datum_cas",
                table: "gdpr_audit_logs",
                columns: new[] { "pojistenec_id", "datum_cas" });

            migrationBuilder.CreateIndex(
                name: "IX_gdpr_audit_logs_uzivatel_id",
                table: "gdpr_audit_logs",
                column: "uzivatel_id");

            migrationBuilder.CreateIndex(
                name: "IX_gdpr_consents_odvolano_kym",
                table: "gdpr_consents",
                column: "odvolano_kym");

            migrationBuilder.CreateIndex(
                name: "IX_gdpr_consents_pojistenec_id_kategorie_je_aktivni",
                table: "gdpr_consents",
                columns: new[] { "pojistenec_id", "kategorie", "je_aktivni" });

            migrationBuilder.CreateIndex(
                name: "IX_gdpr_consents_udeljen_kym",
                table: "gdpr_consents",
                column: "udeljen_kym");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gdpr_audit_logs");

            migrationBuilder.DropTable(
                name: "gdpr_consents");

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
        }
    }
}
