using DataAccessLayer.Abstract;
using DataAccessLayer.Repositories;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class EfFavoriDal : GenericRepository<Favori>, IFavoriDal
    {
        public EfFavoriDal(Context context) : base(context)
        {
        }

        public async Task<int> GetFavoriCountByIlanIdAsync(int ilanId)
        {
            return await _context.Favoriler
                         .CountAsync(x => x.IlanId == ilanId);

        }

        public async Task<List<Favori>> GetFavoriListByUserWithIlanAsync(string userId, CancellationToken ct = default)
        {
            return await _context.Favoriler
                            .Include(x => x.Ilan)
                            .ThenInclude(y => y.Fotografler) 
                            .Include(x => x.Ilan.Kategori)       
                            .Where(x => x.KullaniciId == userId)
                            .OrderByDescending(x => x.IlanId)       
                            .AsNoTracking()                      
                            .ToListAsync(ct);
        }

        public async Task<bool> IsIlanFavoritedByUserAsync(int ilanId, string userId)
        {
            return await _context.Favoriler
                         .AnyAsync(x => x.IlanId == ilanId && x.KullaniciId == userId);
        }

        public async Task<Favori?> GetByUserAndListingAsync(string userId, int ilanId, CancellationToken ct)
        {
            return await _context.Favoriler
                .FirstOrDefaultAsync(x => x.KullaniciId == userId && x.IlanId == ilanId, ct);
        }

        public async Task<bool> ExistsAsync(string userId, int ilanId, CancellationToken ct)
        {
            return await _context.Favoriler
                .AnyAsync(x => x.KullaniciId == userId && x.IlanId == ilanId, ct);
        }
        public async Task<EntityLayer.DTOs.Public.PagedResult<DataAccessLayer.Projections.FavoriteListingProjection>> GetUserFavoritesAsync(string userId, int page, int pageSize, CancellationToken ct)
        {
            var query = _context.Favoriler
                .AsNoTracking()
                .Where(x => x.KullaniciId == userId && !x.Ilan.SilindiMi && x.Ilan.Durum == EntityLayer.Enums.IlanDurumu.Yayinda)
                .Select(x => new DataAccessLayer.Projections.FavoriteListingProjection
                {
                    IlanId = x.Ilan.Id,
                    Baslik = x.Ilan.Baslik,
                    SeoSlug = x.Ilan.SeoSlug,
                    Fiyat = x.Ilan.Fiyat,
                    Sehir = x.Ilan.Sehir,
                    OlusturmaTarihi = x.Ilan.OlusturmaTarihi,
                    KapakFotoUrl = x.Ilan.Fotografler.OrderBy(f => f.SiraNo).Select(f => f.DosyaYolu).FirstOrDefault(),
                    KategoriAdi = x.Ilan.Kategori.Ad
                });

            
            var totalCount = await query.CountAsync(ct);
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

            return new EntityLayer.DTOs.Public.PagedResult<DataAccessLayer.Projections.FavoriteListingProjection>(items, totalCount, page, pageSize);
        }
    }
}
