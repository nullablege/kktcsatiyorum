using DataAccessLayer.Abstract;
using DataAccessLayer.Repositories;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class EfIlanSikayetiDal : GenericRepository<IlanSikayeti>, IIlanSikayetiDal
    {
        public EfIlanSikayetiDal(Context context) : base(context)
        {
        }

        public async Task<List<IlanSikayeti>> GetSikayetListWithIlanAndUserAsync(Expression<Func<IlanSikayeti, bool>>? filter = null)
        {
            var query = _context.IlanSikayetleri
                                .Include(x => x.Ilan)
                                .Include(x => x.SikayetEdenKullanici)
                                .AsNoTracking();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.OrderByDescending(x => x.Id)
                              .ToListAsync();
        }
    }
}
