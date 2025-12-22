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
        Task<List<Kategori>> GetKategoriListWithSubCategoriesAsync();

        Task<Kategori> GetKategoriByIdWithOzelliklerAsync(int id);

    }
}
