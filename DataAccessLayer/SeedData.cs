using DataAccessLayer.Concrete;
using EntityLayer.Entities;
using EntityLayer.Enums;
using EntityLayer.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, bool isDevelopment)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Context>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UygulamaKullanicisi>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (isDevelopment) 
            {
                await context.Database.MigrateAsync();
            }

            // Always ensure roles exist
            await EnsureRoles(roleManager);

            // Seed demo data only in Development
            if (isDevelopment)
            {
                var users = await EnsureUsers(userManager);
                await EnsureCategories(context);
                await EnsureListings(context, users);
            }
        }

        private static async Task EnsureRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(RoleNames.Admin)) await roleManager.CreateAsync(new IdentityRole(RoleNames.Admin));
            if (!await roleManager.RoleExistsAsync(RoleNames.User)) await roleManager.CreateAsync(new IdentityRole(RoleNames.User));
            if (!await roleManager.RoleExistsAsync(RoleNames.Moderator)) await roleManager.CreateAsync(new IdentityRole(RoleNames.Moderator));
        }

        private static async Task<Dictionary<string, UygulamaKullanicisi>> EnsureUsers(UserManager<UygulamaKullanicisi> userManager)
        {
            var userMap = new Dictionary<string, UygulamaKullanicisi>();

            var adminEmail = "admin@kktcsatiyorum.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new UygulamaKullanicisi
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    AdSoyad = "System Admin",
                    EmailConfirmed = true,
                    OlusturmaTarihi = DateTime.UtcNow
                };
                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, RoleNames.Admin);
                }
            }
            userMap[RoleNames.Admin] = adminUser;

            var userEmail = "user@kktcsatiyorum.com";
            var normalUser = await userManager.FindByEmailAsync(userEmail);
            if (normalUser == null)
            {
                normalUser = new UygulamaKullanicisi
                {
                    UserName = userEmail,
                    Email = userEmail,
                    AdSoyad = "Demo User",
                    EmailConfirmed = true,
                    OlusturmaTarihi = DateTime.UtcNow
                };
                var result = await userManager.CreateAsync(normalUser, "User123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(normalUser, RoleNames.User); 
                }
            }
            userMap[RoleNames.User] = normalUser;

            return userMap;
        }

        private static async Task EnsureCategories(Context context)
        {
            if (await context.Kategoriler.AnyAsync()) return;

            var emlak = new Kategori { Ad = "Emlak", SeoSlug = "emlak", SiraNo = 1, AktifMi = true };
            var vasita = new Kategori { Ad = "Vasıta", SeoSlug = "vasita", SiraNo = 2, AktifMi = true };
            var elektronik = new Kategori { Ad = "Elektronik", SeoSlug = "elektronik", SiraNo = 3, AktifMi = true };
            
            context.Kategoriler.AddRange(emlak, vasita, elektronik);
            await context.SaveChangesAsync();

            var satilikDaire = new Kategori { Ad = "Satılık Daire", SeoSlug = "satilik-daire", UstKategoriId = emlak.Id, SiraNo = 1, AktifMi = true };
            var kiralikDaire = new Kategori { Ad = "Kiralık Daire", SeoSlug = "kiralik-daire", UstKategoriId = emlak.Id, SiraNo = 2, AktifMi = true };
            var arsa = new Kategori { Ad = "Arsa", SeoSlug = "arsa", UstKategoriId = emlak.Id, SiraNo = 3, AktifMi = true };
            var isyeri = new Kategori { Ad = "İşyeri", SeoSlug = "isyeri", UstKategoriId = emlak.Id, SiraNo = 4, AktifMi = true };

            var otomobil = new Kategori { Ad = "Otomobil", SeoSlug = "otomobil", UstKategoriId = vasita.Id, SiraNo = 1, AktifMi = true };
            var motosiklet = new Kategori { Ad = "Motosiklet", SeoSlug = "motosiklet", UstKategoriId = vasita.Id, SiraNo = 2, AktifMi = true };

            var telefon = new Kategori { Ad = "Telefon", SeoSlug = "telefon", UstKategoriId = elektronik.Id, SiraNo = 1, AktifMi = true };
            var bilgisayar = new Kategori { Ad = "Bilgisayar", SeoSlug = "bilgisayar", UstKategoriId = elektronik.Id, SiraNo = 2, AktifMi = true };

            context.Kategoriler.AddRange(satilikDaire, kiralikDaire, arsa, isyeri, otomobil, motosiklet, telefon, bilgisayar);
            await context.SaveChangesAsync();

            var m2Alan = new KategoriAlani { KategoriId = emlak.Id, Ad = "Metrekare", Anahtar = "metrekare", VeriTipi = VeriTipi.TamSayi, ZorunluMu = true };
            var odaSayisi = new KategoriAlani { KategoriId = emlak.Id, Ad = "Oda Sayısı", Anahtar = "oda-sayisi", VeriTipi = VeriTipi.Metin, ZorunluMu = true };
            
            context.KategoriAlanlari.AddRange(m2Alan, odaSayisi);
            await context.SaveChangesAsync();
        }

        private static async Task EnsureListings(Context context, Dictionary<string, UygulamaKullanicisi> users)
        {
            if (await context.Ilanlar.AnyAsync()) return;

            var user = users[RoleNames.User]; 
            var admin = users[RoleNames.Admin]; 

            var satilikDaire = await context.Kategoriler.FirstOrDefaultAsync(k => k.SeoSlug == "satilik-daire");
            var isyeri = await context.Kategoriler.FirstOrDefaultAsync(k => k.SeoSlug == "isyeri");
            var otomobil = await context.Kategoriler.FirstOrDefaultAsync(k => k.SeoSlug == "otomobil");
            var telefon = await context.Kategoriler.FirstOrDefaultAsync(k => k.SeoSlug == "telefon");
            var bilgisayar = await context.Kategoriler.FirstOrDefaultAsync(k => k.SeoSlug == "bilgisayar");

            // Safe guard: if critical categories missing, abort seeding
            if (satilikDaire == null || isyeri == null || otomobil == null || telefon == null || bilgisayar == null) return;

            var listings = new List<Ilan>
            {
                new Ilan
                {
                    Baslik = "Girne Merkezde Lüks 3+1 Daire",
                    SeoSlug = "girne-merkezde-luks-3-1-daire",
                    Aciklama = "Girne'nin kalbinde, tüm olanaklara yürüme mesafesinde, harika manzaralı lüks daire.",
                    Fiyat = 150000,
                    ParaBirimi = ParaBirimi.GBP,
                    KategoriId = satilikDaire.Id,
                    SahipKullaniciId = user.Id,
                    Sehir = "Girne",
                    Ilce = "Merkez",
                    Enlem = 35.3354m,
                    Boylam = 33.3184m, // Girne Merkez
                    Durum = IlanDurumu.Yayinda,
                    OlusturmaTarihi = DateTime.UtcNow.AddDays(-5),
                    YayinTarihi = DateTime.UtcNow.AddDays(-4),
                    OnaylayanKullaniciId = admin.Id,
                    OnayTarihi = DateTime.UtcNow.AddDays(-4),
                    SilindiMi = false
                },
                new Ilan
                {
                    Baslik = "Lefkoşa Gönyeli'de Uygun Fiyatlı 2+1",
                    SeoSlug = "lefkosa-gonyeli-de-uygun-fiyatli-2-1",
                    Aciklama = "Öğrenciye veya çalışana uygun, temiz, masrafsız daire.",
                    Fiyat = 75000,
                    ParaBirimi = ParaBirimi.GBP,
                    KategoriId = satilikDaire.Id,
                    SahipKullaniciId = user.Id,
                    Sehir = "Lefkoşa",
                    Ilce = "Gönyeli",
                    Enlem = 35.2163m,
                    Boylam = 33.3088m, // Gönyeli
                    Durum = IlanDurumu.Yayinda,
                    OlusturmaTarihi = DateTime.UtcNow.AddDays(-10),
                    YayinTarihi = DateTime.UtcNow.AddDays(-9),
                    OnaylayanKullaniciId = admin.Id,
                    OnayTarihi = DateTime.UtcNow.AddDays(-9),
                    SilindiMi = false
                },
                new Ilan
                {
                    Baslik = "Mağusa'da Yatırımlık Dükkan",
                    SeoSlug = "magusa-da-yatirimlik-dukkan",
                    Aciklama = "Üniversite yolunda, yüksek kira getirili dükkan.",
                    Fiyat = 90000,
                    ParaBirimi = ParaBirimi.GBP,
                    KategoriId = isyeri.Id,
                    SahipKullaniciId = user.Id,
                    Sehir = "Gazimağusa",
                    Ilce = "Merkez",
                    Enlem = 35.1250m,
                    Boylam = 33.9350m, // Mağusa Merkez
                    Durum = IlanDurumu.OnayBekliyor,
                    OlusturmaTarihi = DateTime.UtcNow,
                    SilindiMi = false
                },
                new Ilan
                {
                    Baslik = "2020 Model Mercedes C200 AMG",
                    SeoSlug = "2020-model-mercedes-c200-amg",
                    Aciklama = "Hatasız, boyasız, tramersiz. Servis bakımlı.",
                    Fiyat = 35000,
                    ParaBirimi = ParaBirimi.GBP,
                    KategoriId = otomobil.Id,
                    SahipKullaniciId = user.Id,
                    Sehir = "Girne",
                    Ilce = "Çatalköy",
                    Enlem = 35.3180m,
                    Boylam = 33.3750m, // Çatalköy
                    Durum = IlanDurumu.Yayinda,
                    OlusturmaTarihi = DateTime.UtcNow.AddDays(-2),
                    YayinTarihi = DateTime.UtcNow.AddDays(-1),
                    OnaylayanKullaniciId = admin.Id,
                    OnayTarihi = DateTime.UtcNow.AddDays(-1),
                    SilindiMi = false
                },
                new Ilan
                {
                    Baslik = "Honda Civic 2018 Eco Elegance",
                    SeoSlug = "honda-civic-2018-eco-elegance",
                    Aciklama = "Aile aracı, temiz kullanılmış.",
                    Fiyat = 650000,
                    ParaBirimi = ParaBirimi.TRY,
                    KategoriId = otomobil.Id,
                    SahipKullaniciId = user.Id,
                    Sehir = "Lefkoşa",
                    Ilce = "Ortaköy",
                    Enlem = 35.1950m,
                    Boylam = 33.3550m, // Ortaköy
                    Durum = IlanDurumu.Yayinda,
                    OlusturmaTarihi = DateTime.UtcNow.AddDays(-20),
                    YayinTarihi = DateTime.UtcNow.AddDays(-19),
                    OnaylayanKullaniciId = admin.Id,
                    OnayTarihi = DateTime.UtcNow.AddDays(-19),
                    SilindiMi = false
                },
                new Ilan
                {
                    Baslik = "iPhone 14 Pro Max 256GB - Garantili",
                    SeoSlug = "iphone-14-pro-max-256gb-garantili",
                    Aciklama = "Kutulu, faturalı, garantisi devam ediyor. Çiziksiz.",
                    Fiyat = 45000,
                    ParaBirimi = ParaBirimi.TRY,
                    KategoriId = telefon.Id,
                    SahipKullaniciId = user.Id,
                    Sehir = "Girne",
                    Ilce = "Karaoğlanoğlu",
                    Enlem = 35.3450m,
                    Boylam = 33.2950m, // Karaoğlanoğlu
                    Durum = IlanDurumu.Yayinda,
                    OlusturmaTarihi = DateTime.UtcNow.AddDays(-1),
                    YayinTarihi = DateTime.UtcNow.AddDays(-1),
                    OnaylayanKullaniciId = admin.Id,
                    OnayTarihi = DateTime.UtcNow.AddDays(-1),
                    SilindiMi = false
                },
                new Ilan
                {
                    Baslik = "Samsung Galaxy S23 Ultra",
                    SeoSlug = "samsung-galaxy-s23-ultra",
                    Aciklama = "Sıfır ayarında, kutu fatura mevcut.",
                    Fiyat = 38000,
                    ParaBirimi = ParaBirimi.TRY,
                    KategoriId = telefon.Id,
                    SahipKullaniciId = user.Id,
                    Sehir = "Lefkoşa",
                    Ilce = "Gönyeli",
                    Enlem = 35.2155m,
                    Boylam = 33.3095m, // Gönyeli
                    Durum = IlanDurumu.Yayinda,
                    OlusturmaTarihi = DateTime.UtcNow.AddDays(-3),
                    YayinTarihi = DateTime.UtcNow.AddDays(-2),
                    OnaylayanKullaniciId = admin.Id,
                    OnayTarihi = DateTime.UtcNow.AddDays(-2),
                    SilindiMi = false
                },
                new Ilan
                {
                    Baslik = "MacBook Air M2 Çip",
                    SeoSlug = "macbook-air-m2-cip",
                    Aciklama = "Çok az kullanıldı, pil döngüsü düşük.",
                    Fiyat = 1100,
                    ParaBirimi = ParaBirimi.EUR,
                    KategoriId = bilgisayar.Id,
                    SahipKullaniciId = user.Id,
                    Sehir = "Lefkoşa",
                    Ilce = "K. Kaymaklı",
                    Durum = IlanDurumu.Taslak,
                    OlusturmaTarihi = DateTime.UtcNow.AddMinutes(-30),
                    SilindiMi = false
                },
                new Ilan
                {
                    Baslik = "Oyuncu Bilgisayarı - RTX 3060",
                    SeoSlug = "oyuncu-bilgisayari-rtx-3060",
                    Aciklama = "Yüksek FPS garantili, tüm oyunları açar.",
                    Fiyat = 25000,
                    ParaBirimi = ParaBirimi.TRY,
                    KategoriId = bilgisayar.Id,
                    SahipKullaniciId = user.Id,
                    Sehir = "Girne",
                    Ilce = "Alsancak",
                    Enlem = 35.3520m,
                    Boylam = 33.2200m, // Alsancak
                    Durum = IlanDurumu.Yayinda,
                    OlusturmaTarihi = DateTime.UtcNow.AddDays(-7),
                    YayinTarihi = DateTime.UtcNow.AddDays(-7),
                    OnaylayanKullaniciId = admin.Id,
                    OnayTarihi = DateTime.UtcNow.AddDays(-7),
                    SilindiMi = false
                },
                new Ilan
                {
                    Baslik = "iPad Pro 12.9 inç M1",
                    SeoSlug = "ipad-pro-12-9-inc-m1",
                    Aciklama = "Magic Keyboard dahil, çizim için ideal.",
                    Fiyat = 950,
                    ParaBirimi = ParaBirimi.GBP,
                    KategoriId = bilgisayar.Id, 
                    SahipKullaniciId = user.Id,
                    Sehir = "Gazimağusa",
                    Ilce = "Gülseren",
                    Enlem = 35.1380m,
                    Boylam = 33.9180m, // Gülseren
                    Durum = IlanDurumu.Yayinda,
                    OlusturmaTarihi = DateTime.UtcNow.AddDays(-14),
                    YayinTarihi = DateTime.UtcNow.AddDays(-13),
                    OnaylayanKullaniciId = admin.Id,
                    OnayTarihi = DateTime.UtcNow.AddDays(-13),
                    SilindiMi = false
                }
            };
            
            context.Ilanlar.AddRange(listings);
            await context.SaveChangesAsync();
        }
    }
}
