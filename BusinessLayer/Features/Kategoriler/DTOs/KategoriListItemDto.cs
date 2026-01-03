using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Features.Kategoriler.DTOs
{
    public sealed record KategoriListItemDto
    {
        public int Id { get; set; }
       
        public string Ad { get; set; } = string.Empty;
        public string SeoSlug { get; set; } = string.Empty;
        public int? UstKategoriId { get; set; }
        public int SiraNo { get; set; }
        public bool AktifMi { get; set; }
    }
}
