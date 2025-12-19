using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class firstMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AdSoyad = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ProfilFotografYolu = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    AskidaMi = table.Column<bool>(type: "bit", nullable: false),
                    AskidaBitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SonGirisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kategoriler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UstKategoriId = table.Column<int>(type: "int", nullable: true),
                    Ad = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SeoSlug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    SiraNo = table.Column<int>(type: "int", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategoriler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kategoriler_Kategoriler_UstKategoriId",
                        column: x => x.UstKategoriId,
                        principalTable: "Kategoriler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bildirimler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Tur = table.Column<short>(type: "smallint", nullable: false),
                    VeriJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OkunduMu = table.Column<bool>(type: "bit", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bildirimler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bildirimler_AspNetUsers_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DenetimKayitlari",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Eylem = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    VarlikAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VarlikId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DetayJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAdresi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DenetimKayitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DenetimKayitlari_AspNetUsers_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Ilanlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SahipKullaniciId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    KategoriId = table.Column<int>(type: "int", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SeoSlug = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fiyat = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ParaBirimi = table.Column<short>(type: "smallint", nullable: false),
                    Durum = table.Column<short>(type: "smallint", nullable: false),
                    RedNedeni = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Sehir = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Ilce = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Enlem = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    Boylam = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    GoruntulenmeSayisi = table.Column<int>(type: "int", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    YayinTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OnaylayanKullaniciId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    OnayTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ilanlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ilanlar_AspNetUsers_OnaylayanKullaniciId",
                        column: x => x.OnaylayanKullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ilanlar_AspNetUsers_SahipKullaniciId",
                        column: x => x.SahipKullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ilanlar_Kategoriler_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "Kategoriler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KategoriAlanlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KategoriId = table.Column<int>(type: "int", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Anahtar = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VeriTipi = table.Column<short>(type: "smallint", nullable: false),
                    ZorunluMu = table.Column<bool>(type: "bit", nullable: false),
                    FiltrelenebilirMi = table.Column<bool>(type: "bit", nullable: false),
                    SiraNo = table.Column<int>(type: "int", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KategoriAlanlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KategoriAlanlari_Kategoriler_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "Kategoriler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favoriler",
                columns: table => new
                {
                    KullaniciId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IlanId = table.Column<int>(type: "int", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favoriler", x => new { x.KullaniciId, x.IlanId });
                    table.ForeignKey(
                        name: "FK_Favoriler_AspNetUsers_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favoriler_Ilanlar_IlanId",
                        column: x => x.IlanId,
                        principalTable: "Ilanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IlanFotograflari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IlanId = table.Column<int>(type: "int", nullable: false),
                    DosyaYolu = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    KapakMi = table.Column<bool>(type: "bit", nullable: false),
                    SiraNo = table.Column<int>(type: "int", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IlanFotograflari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IlanFotograflari_Ilanlar_IlanId",
                        column: x => x.IlanId,
                        principalTable: "Ilanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IlanSikayetleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IlanId = table.Column<int>(type: "int", nullable: false),
                    SikayetEdenKullaniciId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Neden = table.Column<short>(type: "smallint", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Durum = table.Column<short>(type: "smallint", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IlanSikayetleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IlanSikayetleri_AspNetUsers_SikayetEdenKullaniciId",
                        column: x => x.SikayetEdenKullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IlanSikayetleri_Ilanlar_IlanId",
                        column: x => x.IlanId,
                        principalTable: "Ilanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IlanSorulari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IlanId = table.Column<int>(type: "int", nullable: false),
                    SoranKullaniciId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SoruMetni = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Durum = table.Column<short>(type: "smallint", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IlanSorulari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IlanSorulari_AspNetUsers_SoranKullaniciId",
                        column: x => x.SoranKullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IlanSorulari_Ilanlar_IlanId",
                        column: x => x.IlanId,
                        principalTable: "Ilanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KategoriAlanSecenekleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KategoriAlaniId = table.Column<int>(type: "int", nullable: false),
                    Deger = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SiraNo = table.Column<int>(type: "int", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KategoriAlanSecenekleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KategoriAlanSecenekleri_KategoriAlanlari_KategoriAlaniId",
                        column: x => x.KategoriAlaniId,
                        principalTable: "KategoriAlanlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IlanCevaplari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoruId = table.Column<int>(type: "int", nullable: false),
                    CevaplayanKullaniciId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CevapMetni = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IlanCevaplari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IlanCevaplari_AspNetUsers_CevaplayanKullaniciId",
                        column: x => x.CevaplayanKullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IlanCevaplari_IlanSorulari_SoruId",
                        column: x => x.SoruId,
                        principalTable: "IlanSorulari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IlanAlanDegerleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IlanId = table.Column<int>(type: "int", nullable: false),
                    KategoriAlaniId = table.Column<int>(type: "int", nullable: false),
                    SecenekId = table.Column<int>(type: "int", nullable: true),
                    MetinDeger = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TamSayiDeger = table.Column<long>(type: "bigint", nullable: true),
                    OndalikDeger = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    DogruYanlisDeger = table.Column<bool>(type: "bit", nullable: true),
                    TarihDeger = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IlanAlanDegerleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IlanAlanDegerleri_Ilanlar_IlanId",
                        column: x => x.IlanId,
                        principalTable: "Ilanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IlanAlanDegerleri_KategoriAlanSecenekleri_SecenekId",
                        column: x => x.SecenekId,
                        principalTable: "KategoriAlanSecenekleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IlanAlanDegerleri_KategoriAlanlari_KategoriAlaniId",
                        column: x => x.KategoriAlaniId,
                        principalTable: "KategoriAlanlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Bildirimler_KullaniciId_OkunduMu",
                table: "Bildirimler",
                columns: new[] { "KullaniciId", "OkunduMu" });

            migrationBuilder.CreateIndex(
                name: "IX_DenetimKayitlari_KullaniciId",
                table: "DenetimKayitlari",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_DenetimKayitlari_OlusturmaTarihi",
                table: "DenetimKayitlari",
                column: "OlusturmaTarihi");

            migrationBuilder.CreateIndex(
                name: "IX_Favoriler_IlanId",
                table: "Favoriler",
                column: "IlanId");

            migrationBuilder.CreateIndex(
                name: "IX_IlanAlanDegerleri_IlanId_KategoriAlaniId",
                table: "IlanAlanDegerleri",
                columns: new[] { "IlanId", "KategoriAlaniId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IlanAlanDegerleri_KategoriAlaniId_OndalikDeger",
                table: "IlanAlanDegerleri",
                columns: new[] { "KategoriAlaniId", "OndalikDeger" });

            migrationBuilder.CreateIndex(
                name: "IX_IlanAlanDegerleri_KategoriAlaniId_SecenekId",
                table: "IlanAlanDegerleri",
                columns: new[] { "KategoriAlaniId", "SecenekId" });

            migrationBuilder.CreateIndex(
                name: "IX_IlanAlanDegerleri_KategoriAlaniId_TamSayiDeger",
                table: "IlanAlanDegerleri",
                columns: new[] { "KategoriAlaniId", "TamSayiDeger" });

            migrationBuilder.CreateIndex(
                name: "IX_IlanAlanDegerleri_SecenekId",
                table: "IlanAlanDegerleri",
                column: "SecenekId");

            migrationBuilder.CreateIndex(
                name: "IX_IlanCevaplari_CevaplayanKullaniciId",
                table: "IlanCevaplari",
                column: "CevaplayanKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_IlanCevaplari_SoruId",
                table: "IlanCevaplari",
                column: "SoruId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IlanFotograflari_IlanId",
                table: "IlanFotograflari",
                column: "IlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Ilanlar_Fiyat",
                table: "Ilanlar",
                column: "Fiyat");

            migrationBuilder.CreateIndex(
                name: "IX_Ilanlar_KategoriId_Durum_OlusturmaTarihi",
                table: "Ilanlar",
                columns: new[] { "KategoriId", "Durum", "OlusturmaTarihi" });

            migrationBuilder.CreateIndex(
                name: "IX_Ilanlar_OnaylayanKullaniciId",
                table: "Ilanlar",
                column: "OnaylayanKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Ilanlar_SahipKullaniciId",
                table: "Ilanlar",
                column: "SahipKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_IlanSikayetleri_IlanId",
                table: "IlanSikayetleri",
                column: "IlanId");

            migrationBuilder.CreateIndex(
                name: "IX_IlanSikayetleri_SikayetEdenKullaniciId",
                table: "IlanSikayetleri",
                column: "SikayetEdenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_IlanSorulari_IlanId",
                table: "IlanSorulari",
                column: "IlanId");

            migrationBuilder.CreateIndex(
                name: "IX_IlanSorulari_SoranKullaniciId",
                table: "IlanSorulari",
                column: "SoranKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_KategoriAlanlari_KategoriId",
                table: "KategoriAlanlari",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_KategoriAlanlari_KategoriId_Anahtar",
                table: "KategoriAlanlari",
                columns: new[] { "KategoriId", "Anahtar" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KategoriAlanSecenekleri_KategoriAlaniId",
                table: "KategoriAlanSecenekleri",
                column: "KategoriAlaniId");

            migrationBuilder.CreateIndex(
                name: "IX_Kategoriler_SeoSlug",
                table: "Kategoriler",
                column: "SeoSlug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kategoriler_UstKategoriId",
                table: "Kategoriler",
                column: "UstKategoriId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Bildirimler");

            migrationBuilder.DropTable(
                name: "DenetimKayitlari");

            migrationBuilder.DropTable(
                name: "Favoriler");

            migrationBuilder.DropTable(
                name: "IlanAlanDegerleri");

            migrationBuilder.DropTable(
                name: "IlanCevaplari");

            migrationBuilder.DropTable(
                name: "IlanFotograflari");

            migrationBuilder.DropTable(
                name: "IlanSikayetleri");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "KategoriAlanSecenekleri");

            migrationBuilder.DropTable(
                name: "IlanSorulari");

            migrationBuilder.DropTable(
                name: "KategoriAlanlari");

            migrationBuilder.DropTable(
                name: "Ilanlar");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Kategoriler");
        }
    }
}
