using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IGenericRepository<T>  where T :class
    {
        Task InsertAsync(T entity);
        Task DeleteAsync(T entity);
        Task UpdateAsync(T entity);
        Task<T?> GetByIdAsync(object id);
        Task<List<T>> GetListAllAsync(Expression<Func<T, bool>>? filter = null);
        Task<bool> AnyAsync(Expression<Func<T, bool>> filter, CancellationToken ct = default);
    }
}
