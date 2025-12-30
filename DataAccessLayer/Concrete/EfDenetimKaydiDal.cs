using DataAccessLayer.Abstract;
using DataAccessLayer;
using DataAccessLayer.Projections;
using EntityLayer.DTOs.Admin;
using EntityLayer.DTOs.Public;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class EfDenetimKaydiDal : IDenetimKaydiDal
    {
        private readonly Context _context;

        public EfDenetimKaydiDal(Context context)
        {
            _context = context;
        }

        public async Task AddAsync(DenetimKaydi entity, CancellationToken ct)
        {
            await _context.DenetimKayitlari.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<PagedResult<DenetimKaydiListProjection>> GetPagedAsync(DenetimKaydiQuery query, CancellationToken ct)
        {
            var dbQuery = _context.DenetimKayitlari.AsNoTracking().AsQueryable();

            // Filtering
            if (query.BaslangicTarihi.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.OlusturmaTarihi >= query.BaslangicTarihi.Value);
            }
            if (query.BitisTarihi.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.OlusturmaTarihi <= query.BitisTarihi.Value);
            }
            if (!string.IsNullOrEmpty(query.Eylem))
            {
                dbQuery = dbQuery.Where(x => x.Eylem == query.Eylem);
            }
            if (!string.IsNullOrEmpty(query.VarlikAdi))
            {
                dbQuery = dbQuery.Where(x => x.VarlikAdi == query.VarlikAdi);
            }
            if (!string.IsNullOrEmpty(query.KullaniciId))
            {
                dbQuery = dbQuery.Where(x => x.KullaniciId == query.KullaniciId);
            }

            dbQuery = dbQuery.OrderByDescending(x => x.OlusturmaTarihi);

            var totalCount = await dbQuery.CountAsync(ct);

            var items = await dbQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(x => new DenetimKaydiListProjection
                {
                    Id = x.Id,
                    OlusturmaTarihi = x.OlusturmaTarihi,
                    Eylem = x.Eylem,
                    VarlikAdi = x.VarlikAdi,
                    VarlikId = x.VarlikId,
                    KullaniciId = x.KullaniciId,
                    KullaniciEmail = x.Kullanici != null ? x.Kullanici.Email : null,
                    IpAdresi = x.IpAdresi
                })
                .ToListAsync(ct);

            return new PagedResult<DenetimKaydiListProjection>(items, totalCount, query.Page, query.PageSize);
        }
    }
}
