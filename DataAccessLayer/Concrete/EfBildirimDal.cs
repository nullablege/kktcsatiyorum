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
    public class EfBildirimDal : GenericRepository<Bildirim>, IBildirimDal
    {
        public EfBildirimDal(Context context) : base(context)
        {
        }

        public async Task<List<Bildirim>> GetOkunmamisBildirimlerByUserIdAsync(string userId, CancellationToken ct = default)
        {
            return await _context.Bildirimler
                            .Where(x => x.KullaniciId == userId && x.OkunduMu == false)
                            .OrderByDescending(x => x.OlusturmaTarihi)
                            .AsNoTracking()
                            .ToListAsync(ct);
        }

        public async Task<int> GetUnreadCountAsync(string userId, CancellationToken ct = default)
        {
            return await _context.Bildirimler
                .AsNoTracking()
                .Where(x => x.KullaniciId == userId && !x.OkunduMu)
                .CountAsync(ct);
        }

        public async Task<EntityLayer.DTOs.Public.PagedResult<Projections.NotificationProjection>> GetUserNotificationsAsync(string userId, int page, int pageSize, CancellationToken ct = default)
        {
            var query = _context.Bildirimler
                .AsNoTracking()
                .Where(x => x.KullaniciId == userId)
                .OrderByDescending(x => x.OlusturmaTarihi)
                .Select(x => new Projections.NotificationProjection
                {
                    Id = x.Id,
                    Tur = x.Tur,
                    Mesaj = x.Mesaj,
                    VeriJson = x.VeriJson,
                    OkunduMu = x.OkunduMu,
                    OlusturmaTarihi = x.OlusturmaTarihi
                });

            var totalCount = await query.CountAsync(ct);
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

            return new EntityLayer.DTOs.Public.PagedResult<Projections.NotificationProjection>(items, totalCount, page, pageSize);
        }
    }
}
