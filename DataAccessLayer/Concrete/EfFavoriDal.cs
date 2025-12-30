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


            // Note: Projection requested "OlusturmaTarihi" in plan under "IlanId, Baslik...". It likely refers to listing creation date.
    }
}
