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

            // --- Distance Logic ---
            var userLat = query.UserLat;
            var userLng = query.UserLng;
            
            // Optimization: If sorting by distance but no max distance specified, apply a default safety bound (e.g. 500km)
            // to prevent calculating Haversine for the entire database.
            int? effectiveMaxDist = query.MaxDistanceKm;
            if (query.SortByDistance && !effectiveMaxDist.HasValue && userLat.HasValue && userLng.HasValue)
            {
                effectiveMaxDist = 500; // Default safety limit
            }

            // Optimization: Bounding Box pre-filter
            if (effectiveMaxDist.HasValue && userLat.HasValue && userLng.HasValue)
            {
                var maxDist = (decimal)effectiveMaxDist.Value;
                // 1 degree latitude ~= 111 km
                var latDiff = maxDist / 111m;
                var latMin = userLat.Value - latDiff;
                var latMax = userLat.Value + latDiff;
                
                var cosLat = Math.Cos((double)userLat.Value * Math.PI / 180.0);
                var lonDiff = (decimal)(maxDist / (decimal)(111.0 * Math.Abs(cosLat) + 0.001)); // avoid div by zero

                var lonMin = userLng.Value - lonDiff;
                var lonMax = userLng.Value + lonDiff;

                q = q.Where(x => x.Enlem >= latMin && x.Enlem <= latMax &&
                                 x.Boylam >= lonMin && x.Boylam <= lonMax);
            }
            
            // Projection with True Haversine Distance
            // Formula: 2 * R * asin(sqrt(a))
            // a = sin²(Δφ/2) + cos φ1 ⋅ cos φ2 ⋅ sin²(Δλ/2)
            var queryWithDist = q.Select(x => new 
            {
                Data = x,
                DistanceKm = (userLat.HasValue && userLng.HasValue && x.Enlem.HasValue && x.Boylam.HasValue) 
                    ? 12742 * Math.Asin(Math.Sqrt(
                        (Math.Sin(((double)x.Enlem.Value * Math.PI / 180.0 - (double)userLat.Value * Math.PI / 180.0) / 2) *
                         Math.Sin(((double)x.Enlem.Value * Math.PI / 180.0 - (double)userLat.Value * Math.PI / 180.0) / 2)) +
                        Math.Cos((double)userLat.Value * Math.PI / 180.0) *
                        Math.Cos((double)x.Enlem.Value * Math.PI / 180.0) *
                        (Math.Sin(((double)x.Boylam.Value * Math.PI / 180.0 - (double)userLng.Value * Math.PI / 180.0) / 2) *
                         Math.Sin(((double)x.Boylam.Value * Math.PI / 180.0 - (double)userLng.Value * Math.PI / 180.0) / 2))
                      ))
                    : (double?)null
            });

            // Apply exact effective MaxDistance filter (covering both explicit query and default safety bound)
            if (effectiveMaxDist.HasValue && userLat.HasValue && userLng.HasValue)
            {
                queryWithDist = queryWithDist.Where(x => x.DistanceKm.HasValue && x.DistanceKm <= effectiveMaxDist.Value);
            }
            // If sorting by distance and using valid coordinates but no effective limit (shouldn't happen with default logic above, but safe overlap), exclude nulls
            else if (query.SortByDistance && userLat.HasValue && userLng.HasValue)
            {
                 queryWithDist = queryWithDist.Where(x => x.DistanceKm.HasValue);
            }

            // Total Count
            var totalCount = await queryWithDist.CountAsync(ct);

            // Sorting
            if (query.SortByDistance && userLat.HasValue && userLng.HasValue)
            {
                // Null-safe sorting: Ensure rows with null distance (if any slipped through) go to end
                queryWithDist = queryWithDist.OrderBy(x => x.DistanceKm ?? double.MaxValue);
            }
            else
            {
                queryWithDist = query.Sort?.ToLowerInvariant() switch
                {
                    "fiyat_artan" => queryWithDist.OrderBy(x => x.Data.Fiyat),
                    "fiyat_azalan" => queryWithDist.OrderByDescending(x => x.Data.Fiyat),
                    "eski" => queryWithDist.OrderBy(x => x.Data.OlusturmaTarihi),
                    _ => queryWithDist.OrderByDescending(x => x.Data.OlusturmaTarihi)
                };
            }

            // Pagination
            var page = Math.Max(1, query.Page);
            var pageSize = Math.Clamp(query.PageSize, 1, 50);
            var skip = (page - 1) * pageSize;

            var items = await queryWithDist
                .Skip(skip)
                .Take(pageSize)
                .Select(x => new ListingCardDto(
                    x.Data.Id,
                    x.Data.SeoSlug,
                    x.Data.Baslik,
                    x.Data.Fiyat,
                    x.Data.ParaBirimi,
                    x.Data.Sehir,
                    x.Data.Fotografler.Where(f => f.KapakMi).Select(f => f.DosyaYolu).FirstOrDefault() 
                        ?? x.Data.Fotografler.OrderBy(f => f.SiraNo).Select(f => f.DosyaYolu).FirstOrDefault(),
                    x.Data.OlusturmaTarihi,
                    x.Data.Kategori.Ad,
                    x.DistanceKm
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

        public async Task<PagedResult<MyListingProjection>> GetUserListingsAsync(string userId, int page, int pageSize, CancellationToken ct = default)
        {
            var baseQuery = _context.Ilanlar
                .Where(x => x.SahipKullaniciId == userId && !x.SilindiMi)
                .AsNoTracking();

            var totalCount = await baseQuery.CountAsync(ct);

            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 50);
            var skip = (page - 1) * pageSize;

            var items = await baseQuery
                .OrderByDescending(x => x.OlusturmaTarihi)
                .Skip(skip)
                .Take(pageSize)
                .Select(x => new MyListingProjection(
                    x.Id,
                    x.Baslik,
                    x.SeoSlug,
                    x.Fiyat,
                    x.ParaBirimi,
                    x.Durum,
                    x.RedNedeni,
                    x.OlusturmaTarihi,
                    x.YayinTarihi,
                    x.Kategori.Ad,
                    x.Fotografler.Where(f => f.KapakMi).Select(f => f.DosyaYolu).FirstOrDefault()
                        ?? x.Fotografler.OrderBy(f => f.SiraNo).Select(f => f.DosyaYolu).FirstOrDefault()
                ))
                .ToListAsync(ct);

            return new PagedResult<MyListingProjection>(items, totalCount, page, pageSize);
        }
    }
}

