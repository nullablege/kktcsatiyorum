using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IIlanSikayetiDal
    {
        Task<List<IlanSikayeti>> GetSikayetListWithIlanAndUserAsync(Expression<Func<IlanSikayeti, bool>>? filter = null);
    }
}
