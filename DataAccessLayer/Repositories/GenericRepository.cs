using DataAccessLayer.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly Context _context;
        public GenericRepository(Context context)
        {
            _context = context;
        }

        public async Task DeleteAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Silinecek kayıt null olamaz!");

            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();

        }

        public async Task<T> GetByIdAsync(object id)
        {
            if( id == null)
                throw new ArgumentNullException(nameof(id), "id parametresi null olamaz!");
            if (id is int intId && intId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID değeri 0'dan büyük olmalıdır.");
            }

            if (id is Guid guidId && guidId == Guid.Empty)
            {
                throw new ArgumentException("Guid ID boş (Empty) olamaz.", nameof(id));
            }

            var element = await _context.Set<T>().FindAsync(id);
            if ( element == null)
            {
                return null;

            }
            return element;

        }

        public async Task<List<T>> GetListAllAsync(Expression<Func<T, bool>>? filter = null)
        {
            var query = _context.Set<T>().AsNoTracking();
            if(filter != null)
            {
                query = query.Where(filter);
            }
            return await query.ToListAsync();
        }

        public async Task InsertAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Eklenecek kayıt null olamaz!");

            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            if(entity == null)
                throw new ArgumentNullException(nameof(entity), "Güncellenecek kayıt null olamaz!");

            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
