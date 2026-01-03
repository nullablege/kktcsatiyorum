using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IKategoriDal:IGenericRepository<Kategori>
    {
        Task<List<Kategori>> GetKategoriListWithSubCategoriesAsync(CancellationToken ct=default);

        Task<Kategori?> GetKategoriByIdWithOzelliklerAsync(int id, CancellationToken ct = default);

        Task<List<Kategori>> GetChildrenAsync(int ustKategoriId, CancellationToken ct = default);
        Task<List<Kategori>> GetRootAsync(CancellationToken ct= default);

        Task<bool> HasChildrenAsync(int id, CancellationToken ct = default);



    }
}
