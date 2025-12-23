using BusinessLayer.Common.Text;
using DataAccessLayer.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Features.Kategoriler.Services
{
    public class KategoriSlugService : IKategoriSlugService
    {
        private readonly IKategoriDal _kategoriDal;

        public KategoriSlugService(IKategoriDal kategoriDal)
        {
             _kategoriDal = kategoriDal;
        }

        public async Task<string> GenerateUniqueAsync(string kategoriAdi, CancellationToken ct)
        {
            var baseSlug = SlugHelper.Slugify(kategoriAdi);

            if (string.IsNullOrEmpty(baseSlug))
                baseSlug = Guid.NewGuid().ToString("n")[..8];

            if (!await _kategoriDal.AnyAsync(x => x.SeoSlug == baseSlug, ct))
                return baseSlug;

            for(var i = 2; i <= 100; i++)
            {
                var slugAday = $"{baseSlug}-{i}";
                if (!await _kategoriDal.AnyAsync(x => x.SeoSlug == slugAday, ct))
                    return slugAday;
            }

            return $"{baseSlug}-{Guid.NewGuid().ToString("n")[..6]}";
        }

    }
}
