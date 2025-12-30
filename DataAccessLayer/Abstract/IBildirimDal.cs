using EntityLayer.Entities;

namespace DataAccessLayer.Abstract
{
    public interface IBildirimDal : IGenericRepository<Bildirim>
    {
        Task<List<Bildirim>> GetOkunmamisBildirimlerByUserIdAsync(string userId, CancellationToken ct = default);
    }
}
