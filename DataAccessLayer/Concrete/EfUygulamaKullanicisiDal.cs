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
    public class EfUygulamaKullanicisiDal : GenericRepository<UygulamaKullanicisi>, IUygulamaKullanicisiDal
    {
        public EfUygulamaKullanicisiDal(Context context) : base(context)
        {
        }
    }
}
