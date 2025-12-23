using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Features.Kategoriler.Services
{
    public interface IKategoriSlugService
    {
        Task<string> GenerateUniqueAsync(string kategoriAdi, CancellationToken ct);
    }
}
