using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Features.Kategoriler.DTOs
{
    public sealed record KategoriDetailDto
    {

        public int Id { get; set; }

        public int? UstKategoriId { get; set; }


        public string Ad { get; set; } = default!;


        public string SeoSlug { get; set; } = default!;

        public bool AktifMi { get; set; } = true;



        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;


        public DateTime? GuncellemeTarihi { get; set; }


        public bool SilindiMi { get; set; }

        public Kategori? UstKategori { get; set; }

    }
}
