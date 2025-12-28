using DataAccessLayer.Abstract;
using DataAccessLayer.Repositories;
using EntityLayer.Entities;

namespace DataAccessLayer.Concrete
{
    public class EfKategoriAlaniSecenegiDal : GenericRepository<KategoriAlaniSecenegi>, IKategoriAlaniSecenegiDal
    {
        public EfKategoriAlaniSecenegiDal(Context context) : base(context)
        {
        }
    }
}
