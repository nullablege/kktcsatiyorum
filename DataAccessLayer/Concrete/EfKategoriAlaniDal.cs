using DataAccessLayer.Abstract;
using DataAccessLayer.Repositories;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Concrete
{
    public class EfKategoriAlaniDal : GenericRepository<KategoriAlani>, IKategoriAlaniDal
    {
        public EfKategoriAlaniDal(Context context) : base(context)
        {
        }

        public async Task<List<KategoriAlani>> GetListByKategoriAsync(int kategoriId, bool includeSecenekler = false, CancellationToken ct = default)
        {
            var query = _context.KategoriAlanlari
                .AsNoTracking()
                .Where(x => x.KategoriId == kategoriId && x.AktifMi)
                .OrderBy(x => x.SiraNo);

            if (includeSecenekler)
            {
                return await query
                    .Include(x => x.Secenekler.Where(s => s.AktifMi).OrderBy(s => s.SiraNo))
                    .ToListAsync(ct);
            }

            return await query.ToListAsync(ct);
        }

        public async Task<KategoriAlani?> GetByIdWithSeceneklerAsync(int id, CancellationToken ct = default)
        {
            return await _context.KategoriAlanlari
                .AsNoTracking()
                .Include(x => x.Kategori)
                .Include(x => x.Secenekler.Where(s => s.AktifMi).OrderBy(s => s.SiraNo))
                .FirstOrDefaultAsync(x => x.Id == id && x.AktifMi, ct);
        }

    }
}
