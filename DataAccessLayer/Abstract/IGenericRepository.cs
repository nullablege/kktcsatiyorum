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
        Task InsertAsync(T entity, CancellationToken ct = default);
        Task DeleteAsync(T entity, CancellationToken ct = default);
        Task UpdateAsync(T entity, CancellationToken ct = default);
        Task<T?> GetByIdAsync(object id, CancellationToken ct = default);
        Task<List<T>> GetListAllAsync(Expression<Func<T, bool>>? filter = null, CancellationToken ct = default);
        Task<bool> AnyAsync(Expression<Func<T, bool>> filter, CancellationToken ct = default);
        Task<int> CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken ct = default);
    }
}
