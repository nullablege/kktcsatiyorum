using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Enums
{
    public enum IlanDurumu : short
    {
        Taslak = 1,
        OnayBekliyor = 2,
        Yayinda = 3,
        Reddedildi = 4,
        Satildi = 5,
        Arsiv = 6
    }
}
