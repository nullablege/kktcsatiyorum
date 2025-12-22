using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IIlanDal:IGenericRepository<Ilan>
    {
        Task<List<Ilan>> GetIlanListWithCategoryAndPhotosAsync(Expression<Func<Ilan, bool>>? filter = null);

        Task<Ilan> GetIlanByIdWithDetailsAsync(int id);

        Task<List<Ilan>> GetIlanListByUserIdAsync(string userId);

    }
}
