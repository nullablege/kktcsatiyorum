using DataAccessLayer.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        public readonly Context _context;
        public UnitOfWork(Context context)
        {
            _context = context;
        }
        public Task<int> CommitAsync(CancellationToken ct = default)
        {
            return _context.SaveChangesAsync(ct);
        }
    }
}
