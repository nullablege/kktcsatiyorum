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

        public async Task<List<Bildirim>> GetOkunmamisBildirimlerByUserIdAsync(string userId)
        {
            return await _context.Bildirimler
                            .Where(x => x.KullaniciId == userId && x.OkunduMu == false)
                            .OrderByDescending(x => x.OlusturmaTarihi)
                            .AsNoTracking()
                            .ToListAsync();
        }
    }
}
