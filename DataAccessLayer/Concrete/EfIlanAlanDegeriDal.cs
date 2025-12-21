using DataAccessLayer.Abstract;
using DataAccessLayer.Repositories;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class EfIlanAlanDegeriDal : GenericRepository<IlanAlanDegeri>, IIlanAlanDegeriDal
    {
        public EfIlanAlanDegeriDal(Context context) : base(context)
        {
        }
    }
}
