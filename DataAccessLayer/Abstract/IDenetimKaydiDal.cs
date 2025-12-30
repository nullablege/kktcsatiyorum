using DataAccessLayer.Requests;
using DataAccessLayer.Projections;
using EntityLayer.DTOs.Public;
using EntityLayer.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IDenetimKaydiDal
    {
        Task<PagedResult<DenetimKaydiListProjection>> GetPagedAsync(DenetimKaydiDalRequest request, CancellationToken ct);
        Task AddAsync(DenetimKaydi entity, CancellationToken ct);
    }
}
