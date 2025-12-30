using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IFavoriDal : IGenericRepository<Favori>
    {
        Task<List<Favori>> GetFavoriListByUserWithIlanAsync(string userId, CancellationToken ct = default);
        Task<int> GetFavoriCountByIlanIdAsync(int ilanId);
        Task<bool> IsIlanFavoritedByUserAsync(int ilanId, string userId);

        Task<Favori?> GetByUserAndListingAsync(string userId, int ilanId, CancellationToken ct);
        Task<bool> ExistsAsync(string userId, int ilanId, CancellationToken ct);
        Task<EntityLayer.DTOs.Public.PagedResult<DataAccessLayer.Projections.FavoriteListingProjection>> GetUserFavoritesAsync(string userId, int page, int pageSize, CancellationToken ct);
    }
}
