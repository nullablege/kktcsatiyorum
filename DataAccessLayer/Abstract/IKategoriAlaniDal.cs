using EntityLayer.Entities;

namespace DataAccessLayer.Abstract
{
    public interface IKategoriAlaniDal : IGenericRepository<KategoriAlani>
    {
        Task<List<KategoriAlani>> GetListByKategoriAsync(int kategoriId, bool includeSecenekler = false, CancellationToken ct = default);
        Task<KategoriAlani?> GetByIdWithSeceneklerAsync(int id, CancellationToken ct = default);
    }
}
