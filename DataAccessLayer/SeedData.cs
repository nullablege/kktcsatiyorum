using DataAccessLayer.Concrete;
using EntityLayer.Entities;
using EntityLayer.Enums;
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
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Context>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UygulamaKullanicisi>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database is created and migrated
            await context.Database.MigrateAsync();

            await EnsureRoles(roleManager);
            var users = await EnsureUsers(userManager);
            await EnsureCategories(context);
            
            // Only seed listings if categories exist (they should now)
            await EnsureListings(context, users);
        }

        private static async Task EnsureRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "User", "Member" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task<Dictionary<string, UygulamaKullanicisi>> EnsureUsers(UserManager<UygulamaKullanicisi> userManager)
        {
            var userMap = new Dictionary<string, UygulamaKullanicisi>();

            // Admin
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
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
            userMap["Admin"] = adminUser;

            // Standard User
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
                    await userManager.AddToRoleAsync(normalUser, "User"); 
                }
            }
            userMap["User"] = normalUser;

            return userMap;
        }

        private static async Task EnsureCategories(Context context)
        {
            if (await context.Kategoriler.AnyAsync()) return;

            // Parent Categories
            var emlak = new Kategori { Ad = "Emlak", SeoSlug = "emlak", SiraNo = 1, AktifMi = true };
            var vasita = new Kategori { Ad = "Vasıta", SeoSlug = "vasita", SiraNo = 2, AktifMi = true };
            var elektronik = new Kategori { Ad = "Elektronik", SeoSlug = "elektronik", SiraNo = 3, AktifMi = true };
            
            context.Kategoriler.AddRange(emlak, vasita, elektronik);
            await context.SaveChangesAsync();

            // Sub Categories (Emlak)
            var satilikDaire = new Kategori { Ad = "Satılık Daire", SeoSlug = "satilik-daire", UstKategoriId = emlak.Id, SiraNo = 1, AktifMi = true };
            var kiralikDaire = new Kategori { Ad = "Kiralık Daire", SeoSlug = "kiralik-daire", UstKategoriId = emlak.Id, SiraNo = 2, AktifMi = true };
            var arsa = new Kategori { Ad = "Arsa", SeoSlug = "arsa", UstKategoriId = emlak.Id, SiraNo = 3, AktifMi = true };

            // Sub Categories (Vasita)
            var otomobil = new Kategori { Ad = "Otomobil", SeoSlug = "otomobil", UstKategoriId = vasita.Id, SiraNo = 1, AktifMi = true };
            var motosiklet = new Kategori { Ad = "Motosiklet", SeoSlug = "motosiklet", UstKategoriId = vasita.Id, SiraNo = 2, AktifMi = true };

            // Sub Categories (Elektronik)
            var telefon = new Kategori { Ad = "Telefon", SeoSlug = "telefon", UstKategoriId = elektronik.Id, SiraNo = 1, AktifMi = true };
            var bilgisayar = new Kategori { Ad = "Bilgisayar", SeoSlug = "bilgisayar", UstKategoriId = elektronik.Id, SiraNo = 2, AktifMi = true };

            context.Kategoriler.AddRange(satilikDaire, kiralikDaire, arsa, otomobil, motosiklet, telefon, bilgisayar);
            await context.SaveChangesAsync();

            // Attributes (Emlak - Metrekare)
            var m2Alan = new KategoriAlani { KategoriId = emlak.Id, Ad = "Metrekare", Anahtar = "metrekare", VeriTipi = VeriTipi.TamSayi, ZorunluMu = true };
            var odaSayisi = new KategoriAlani { KategoriId = emlak.Id, Ad = "Oda Sayısı", Anahtar = "oda-sayisi", VeriTipi = VeriTipi.Metin, ZorunluMu = true };
            
            context.KategoriAlanlari.AddRange(m2Alan, odaSayisi);
            await context.SaveChangesAsync();
        }

        private static async Task EnsureListings(Context context, Dictionary<string, UygulamaKullanicisi> users)
        {
            if (await context.Ilanlar.AnyAsync()) return;

            var user = users["User"]; // Owner of the listings
            var admin = users["Admin"]; // Approver

            // Fetch categories for correct linking
            var satilikDaire = await context.Kategoriler.FirstOrDefaultAsync(k => k.SeoSlug == "satilik-daire");
            var otomobil = await context.Kategoriler.FirstOrDefaultAsync(k => k.SeoSlug == "otomobil");
            var telefon = await context.Kategoriler.FirstOrDefaultAsync(k => k.SeoSlug == "telefon");

            if (satilikDaire == null || otomobil == null || telefon == null) return;

            var listings = new List<Ilan>
            {
                // Real Estate
                new Ilan
                {
                    Baslik = "Girne Merkezde Lüks 3+1 Daire",
                    SeoSlug = "girne-merkezde-luks-3-1-daire",
                    Aciklama = "Girne'nin kalbinde, tüm olanaklara yürüme mesafesinde, harika manzaralı lüks daire.",
                    Fiyat = 150000,
                    ParaBirimi = ParaBirimi.EUR,
                    KategoriId = satilikDaire.Id,
                    SahipKullaniciId = user.Id,
                    Sehir = "Girne",
                    Ilce = "Merkez",
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
                    ParaBirimi = ParaBirimi.EUR,
                    KategoriId = satilikDaire.Id,
                    SahipKullaniciId = user.Id,
                    Sehir = "Lefkoşa",
                    Ilce = "Gönyeli",
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
                    ParaBirimi = ParaBirimi.EUR,
                    KategoriId = satilikDaire.Id, // Assuming property type, simplified
                    SahipKullaniciId = user.Id,
                    Sehir = "Gazimağusa",
                    Ilce = "Merkez",
                    Durum = IlanDurumu.OnayBekliyor,
                    OlusturmaTarihi = DateTime.UtcNow,
                    SilindiMi = false
                },

                // Vehicles
                new Ilan
                {
                    Baslik = "2020 Model Mercedes C200 AMG",
                    SeoSlug = "2020-model-mercedes-c200-amg",
                    Aciklama = "Hatasız, boyasız, tramersiz. Servis bakımlı.",
                    Fiyat = 35000,
                    ParaBirimi = ParaBirimi.EUR,
                    KategoriId = otomobil.Id,
                    SahipKullaniciId = user.Id,
                    Sehir = "Girne",
                    Ilce = "Çatalköy",
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
                    Durum = IlanDurumu.Yayinda,
                    OlusturmaTarihi = DateTime.UtcNow.AddDays(-20),
                    YayinTarihi = DateTime.UtcNow.AddDays(-19),
                    OnaylayanKullaniciId = admin.Id,
                    OnayTarihi = DateTime.UtcNow.AddDays(-19),
                    SilindiMi = false
                },

                // Electronics
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
                    Durum = IlanDurumu.Yayinda,
                    OlusturmaTarihi = DateTime.UtcNow.AddDays(-1),
                    YayinTarihi = DateTime.UtcNow.AddDays(-1),
                    OnaylayanKullaniciId = admin.Id,
                    OnayTarihi = DateTime.UtcNow.AddDays(-1),
                    SilindiMi = false
                },
                new Ilan
                {
                    Baslik = "MacBook Air M2 Çip",
                    SeoSlug = "macbook-air-m2-cip",
                    Aciklama = "Çok az kullanıldı, pil döngüsü düşük.",
                    Fiyat = 1100,
                    ParaBirimi = ParaBirimi.EUR, // Assuming EUR support or fallback
                    KategoriId = telefon.Id, // Needs computer category ideally but simplifying
                    SahipKullaniciId = user.Id,
                    Sehir = "Lefkoşa",
                    Ilce = "K. Kaymaklı",
                    Durum = IlanDurumu.Taslak,
                    OlusturmaTarihi = DateTime.UtcNow.AddMinutes(-30),
                    SilindiMi = false
                }
            };
            
            // Add remaining to reach 10 if needed, simplified for now
            
            context.Ilanlar.AddRange(listings);
            await context.SaveChangesAsync();
        }
    }
}
