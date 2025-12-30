using DataAccessLayer.Abstract;
using DataAccessLayer.Projections;
using DataAccessLayer.Repositories;
using EntityLayer.DTOs.Public;
using EntityLayer.Entities;
using EntityLayer.Enums;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq.Expressions;

namespace DataAccessLayer.Concrete
{
    public class EfIlanDal : GenericRepository<Ilan>, IIlanDal
    {
        public EfIlanDal(Context context) : base(context)
        {
        }

        public async Task<Ilan> GetIlanByIdWithDetailsAsync(int id, CancellationToken ct = default)
        {
            return await _context.Ilanlar
                         .Include(x => x.Kategori)       
                         .Include(x => x.SahipKullanici)
                         .Include(x => x.Fotografler)   
                         .Include(x => x.AlanDegerleri)
                         .AsNoTracking()
                         .FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KeyNotFoundException($"Ilan bulunamadı. Id={id}");
        }

        public async Task<List<Ilan>> GetIlanListByUserIdAsync(string userId, CancellationToken ct = default)
        {
            return await _context.Ilanlar
                        .Include(x => x.Kategori)
                        .Include(x => x.SahipKullanici)
                        .Include(x => x.Fotografler)
                        .Include(x => x.AlanDegerleri)
                        .Where(x => x.SahipKullaniciId == userId)
                        .AsNoTracking()
                        .ToListAsync(ct);
        }

        public async Task<List<Ilan>> GetIlanListWithCategoryAndPhotosAsync(Expression<Func<Ilan, bool>>? filter = null, CancellationToken ct = default)
        {
            var query = _context.Ilanlar
                         .Include(x => x.Kategori)
                         .Include(x => x.Fotografler)
                         .AsNoTracking();
            if (filter != null)
                query = query.Where(filter);

            return await query.ToListAsync(ct);
        }

        public async Task<PagedResult<ListingCardDto>> SearchPublicAsync(ListingSearchQuery query, CancellationToken ct = default)
        {
            var q = _context.Ilanlar
                .Include(x => x.Kategori)
                .Include(x => x.Fotografler)
                .Where(x => x.Durum == IlanDurumu.Yayinda && !x.SilindiMi)
                .AsNoTracking();

            // Text search
            if (!string.IsNullOrWhiteSpace(query.Q))
            {
                var searchTerm = query.Q.ToLowerInvariant();
                q = q.Where(x => x.Baslik.ToLower().Contains(searchTerm) || 
                                 (x.Aciklama != null && x.Aciklama.ToLower().Contains(searchTerm)));
            }

            // Category filter
            if (query.KategoriId.HasValue && query.KategoriId > 0)
                q = q.Where(x => x.KategoriId == query.KategoriId.Value);

            // Price range
            if (query.MinFiyat.HasValue)
                q = q.Where(x => x.Fiyat >= query.MinFiyat.Value);
            if (query.MaxFiyat.HasValue)
                q = q.Where(x => x.Fiyat <= query.MaxFiyat.Value);

            // City filter
            if (!string.IsNullOrWhiteSpace(query.Sehir))
                q = q.Where(x => x.Sehir == query.Sehir);

            // EAV filters
            if (query.EavFilters != null && query.EavFilters.Count > 0)
            {
                foreach (var eav in query.EavFilters)
                {
                    var attrId = eav.Key;
                    var attrValue = eav.Value;
                    if (string.IsNullOrWhiteSpace(attrValue)) continue;

                    q = q.Where(x => x.AlanDegerleri.Any(ad => 
                        ad.KategoriAlaniId == attrId && 
                        (ad.MetinDeger == attrValue || 
                         ad.SecenekId.ToString() == attrValue ||
                         ad.TamSayiDeger.ToString() == attrValue)));
                }
            }

            // Count before pagination
            var totalCount = await q.CountAsync(ct);

            // Sorting
            q = query.Sort?.ToLowerInvariant() switch
            {
                "fiyat_artan" => q.OrderBy(x => x.Fiyat),
                "fiyat_azalan" => q.OrderByDescending(x => x.Fiyat),
                "eski" => q.OrderBy(x => x.OlusturmaTarihi),
                _ => q.OrderByDescending(x => x.OlusturmaTarihi)
            };

            // Pagination
            var page = Math.Max(1, query.Page);
            var pageSize = Math.Clamp(query.PageSize, 1, 50);
            var skip = (page - 1) * pageSize;

            var items = await q
                .Skip(skip)
                .Take(pageSize)
                .Select(x => new ListingCardDto(
                    x.Id,
                    x.SeoSlug,
                    x.Baslik,
                    x.Fiyat,
                    x.ParaBirimi,
                    x.Sehir,
                    x.Fotografler.Where(f => f.KapakMi).Select(f => f.DosyaYolu).FirstOrDefault() 
                        ?? x.Fotografler.OrderBy(f => f.SiraNo).Select(f => f.DosyaYolu).FirstOrDefault(),
                    x.OlusturmaTarihi,
                    x.Kategori.Ad
                ))
                .ToListAsync(ct);

            return new PagedResult<ListingCardDto>(items, totalCount, page, pageSize);
        }

        public async Task<ListingDetailDto?> GetPublicDetailBySlugAsync(string slug, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return null;

            var ilan = await _context.Ilanlar
                .Include(x => x.Fotografler)
                .Include(x => x.AlanDegerleri)
                    .ThenInclude(ad => ad.KategoriAlani)
                .Include(x => x.AlanDegerleri)
                    .ThenInclude(ad => ad.Secenek)
                .Include(x => x.SahipKullanici)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.SeoSlug == slug && x.Durum == IlanDurumu.Yayinda && !x.SilindiMi, ct);

            if (ilan == null)
                return null;

            var fotograflar = ilan.Fotografler
                .OrderBy(f => f.SiraNo)
                .Select(f => new PhotoDto(f.DosyaYolu, f.KapakMi, f.SiraNo))
                .ToList();

            var attributes = ilan.AlanDegerleri
                .Where(ad => ad.KategoriAlani != null)
                .OrderBy(ad => ad.KategoriAlani!.SiraNo)
                .Select(ad => new AttributeValueDisplayDto(
                    ad.KategoriAlani!.Ad,
                    FormatAttributeValue(ad)
                ))
                .ToList();

            return new ListingDetailDto(
                ilan.Id,
                ilan.SeoSlug,
                ilan.Baslik,
                ilan.Aciklama,
                ilan.Fiyat,
                ilan.ParaBirimi,
                ilan.Sehir,
                ilan.OlusturmaTarihi,
                ilan.SahipKullanici?.AdSoyad,
                fotograflar,
                attributes
            );
        }

        private static string FormatAttributeValue(IlanAlanDegeri ad)
        {
            if (ad.Secenek != null)
                return ad.Secenek.Deger;
            if (!string.IsNullOrEmpty(ad.MetinDeger))
                return ad.MetinDeger;
            if (ad.TamSayiDeger.HasValue)
                return ad.TamSayiDeger.Value.ToString("N0", CultureInfo.GetCultureInfo("tr-TR"));
            if (ad.OndalikDeger.HasValue)
                return ad.OndalikDeger.Value.ToString("N2", CultureInfo.GetCultureInfo("tr-TR"));
            if (ad.DogruYanlisDeger.HasValue)
                return ad.DogruYanlisDeger.Value ? "Evet" : "Hayır";
            if (ad.TarihDeger.HasValue)
                return ad.TarihDeger.Value.ToString("dd.MM.yyyy");
            return "-";
        }

        public async Task<PagedResult<PendingListingProjection>> GetPendingApprovalsAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var baseQuery = _context.Ilanlar
                .Where(x => x.Durum == IlanDurumu.OnayBekliyor && !x.SilindiMi)
                .AsNoTracking();

            var totalCount = await baseQuery.CountAsync(ct);

            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 50);
            var skip = (page - 1) * pageSize;

            var items = await baseQuery
                .OrderByDescending(x => x.OlusturmaTarihi)
                .Skip(skip)
                .Take(pageSize)
                .Select(x => new PendingListingProjection(
                    x.Id,
                    x.Baslik,
                    x.SeoSlug,
                    x.Fiyat,
                    x.ParaBirimi,
                    x.Sehir,
                    x.OlusturmaTarihi,
                    x.Kategori.Ad,
                    x.SahipKullaniciId,
                    x.SahipKullanici.AdSoyad ?? x.SahipKullanici.UserName ?? "",
                    x.SahipKullanici.Email,
                    x.Fotografler.Where(f => f.KapakMi).Select(f => f.DosyaYolu).FirstOrDefault()
                        ?? x.Fotografler.OrderBy(f => f.SiraNo).Select(f => f.DosyaYolu).FirstOrDefault()
                ))
                .ToListAsync(ct);

            return new PagedResult<PendingListingProjection>(items, totalCount, page, pageSize);
        }
    }
}

