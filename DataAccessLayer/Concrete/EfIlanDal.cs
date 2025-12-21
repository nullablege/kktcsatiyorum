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
    public class EfIlanDal : GenericRepository<Ilan>, IIlanDal
    {
        public EfIlanDal(Context context) : base(context)
        {
        }

        public async Task<Ilan> GetIlanByIdWithDetailsAsync(int id)
        {
            return await _context.Ilanlar
                         .Include(x => x.Kategori)       
                         .Include(x => x.SahipKullanici)
                         .Include(x => x.Fotografler)   
                         .Include(x => x.AlanDegerleri)
                         .AsNoTracking()
                         .FirstOrDefaultAsync(x => x.Id == id) ?? throw new KeyNotFoundException($"Ilan bulunamadı. Id={id}");
        }

        public async Task<List<Ilan>> GetIlanListByUserIdAsync(string userId)
        {
            return await _context.Ilanlar
                        .Include(x => x.Kategori)
                        .Include(x => x.SahipKullanici)
                        .Include(x => x.Fotografler)
                        .Include(x => x.AlanDegerleri)
                        .Where(x => x.SahipKullaniciId == userId)
                        .AsNoTracking()
                        .ToListAsync();
        }

        public async Task<List<Ilan>> GetIlanListWithCategoryAndPhotosAsync(Expression<Func<Ilan, bool>>? filter = null)
        {
            var query = _context.Ilanlar
                         .Include(x => x.Kategori)
                         .Include(x => x.Fotografler)
                         .AsNoTracking();
            if(filter != null)
                query = query.Where(filter);

            return await query.ToListAsync();
        }
    }
}
