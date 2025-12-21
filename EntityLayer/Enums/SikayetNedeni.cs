using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Enums
{
    public enum SikayetNedeni : short
    {
        Spam = 1,
        Dolandiricilik = 2,
        UygunsuzIcerik = 3,
        YanlisKategori = 4,
        Diger = 99
    }
}
