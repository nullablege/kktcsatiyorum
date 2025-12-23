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
    public class EfKategoriDal : GenericRepository<Kategori>, IKategoriDal
    {
        public EfKategoriDal(Context context) : base(context)
        {
        }

        public async Task<Kategori> GetKategoriByIdWithOzelliklerAsync(int id, CancellationToken ct = default)
        {
            return await _context.Kategoriler
                                 .Include(x => x.UstKategori)
                                 .Include(x => x.AltKategoriler)
                                 .Include(x => x.KategoriAlanlari)
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(x => x.Id == id,ct);
        }

        public async Task<List<Kategori>> GetKategoriListWithSubCategoriesAsync(CancellationToken ct = default)
        {
            return await _context.Kategoriler
                                 .Include(x => x.AltKategoriler)
                                 .Where( x => x.UstKategoriId  == null)
                                 .AsNoTracking()
                                 .OrderByDescending(x => x.Id)
                                 .ToListAsync(ct);
        }
    }
}
