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
    public class EfIlanSorusuDal : GenericRepository<IlanSorusu>, IIlanSorusuDal
    {
        public EfIlanSorusuDal(Context context) : base(context)
        {
        }

        public async Task<List<IlanSorusu>> GetSorularByIlanIdWithCevaplarAsync(int ilanId, CancellationToken ct = default)
        {
            return await _context.IlanSorulari
                                 .Include(x => x.SoranKullanici)
                                 .Include(x => x.Cevap)
                                 .Where(x => x.IlanId == ilanId)
                                 .OrderByDescending(x => x.OlusturmaTarihi)
                                 .AsNoTracking()
                                 .ToListAsync(ct);
        }
    }
}
