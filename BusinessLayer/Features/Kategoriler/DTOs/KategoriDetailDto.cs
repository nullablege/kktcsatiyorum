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

        public int Id { get; init; }

        public int? UstKategoriId { get; init; }


        public string Ad { get; init; } = default!;


        public string SeoSlug { get; init; } = default!;

        public bool AktifMi { get; init; } = true;



        public DateTime OlusturmaTarihi { get; init; } = DateTime.UtcNow;


        public DateTime? GuncellemeTarihi { get; init; }


        public bool SilindiMi { get; init; }


    }
}
