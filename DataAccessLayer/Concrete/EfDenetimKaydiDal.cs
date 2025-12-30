using DataAccessLayer.Abstract;
using DataAccessLayer;
using DataAccessLayer.Projections;
using DataAccessLayer.Requests;
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
        }

        public async Task<PagedResult<DenetimKaydiListProjection>> GetPagedAsync(DenetimKaydiDalRequest request, CancellationToken ct)
        {
            var dbQuery = _context.DenetimKayitlari.AsNoTracking().AsQueryable();

            // Filtering
            if (request.BaslangicTarihi.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.OlusturmaTarihi >= request.BaslangicTarihi.Value);
            }
            if (request.BitisTarihi.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.OlusturmaTarihi <= request.BitisTarihi.Value);
            }
            if (!string.IsNullOrEmpty(request.Eylem))
            {
                dbQuery = dbQuery.Where(x => x.Eylem == request.Eylem);
            }
            if (!string.IsNullOrEmpty(request.VarlikAdi))
            {
                dbQuery = dbQuery.Where(x => x.VarlikAdi == request.VarlikAdi);
            }
            if (!string.IsNullOrEmpty(request.KullaniciId))
            {
                dbQuery = dbQuery.Where(x => x.KullaniciId == request.KullaniciId);
            }

            // Ordering
            dbQuery = dbQuery.OrderByDescending(x => x.OlusturmaTarihi);

            var totalCount = await dbQuery.CountAsync(ct);

            var items = await dbQuery
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
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

            return new PagedResult<DenetimKaydiListProjection>(items, totalCount, request.Page, request.PageSize);
        }
    }
}
