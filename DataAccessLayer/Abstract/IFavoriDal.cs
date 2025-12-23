using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IFavoriDal
    {
        Task<List<Favori>> GetFavoriListByUserWithIlanAsync(string userId, CancellationToken ct = default);
        Task<int> GetFavoriCountByIlanIdAsync(int ilanId);
        Task<bool> IsIlanFavoritedByUserAsync(int ilanId, string userId);
    }
}
