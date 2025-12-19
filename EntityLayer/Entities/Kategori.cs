using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    [Table("Kategoriler")]
    public class Kategori
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("UstKategoriId")]
        public int? UstKategoriId { get; set; }

        [Column("Ad")]
        [MaxLength(150)]
        public string Ad { get; set; } = default!;

        [Column("SeoSlug")]
        [MaxLength(200)]
        public string SeoSlug { get; set; } = default!;

        [Column("AktifMi")]
        public bool AktifMi { get; set; } = true;

        [Column("SiraNo")]
        public int SiraNo { get; set; }

        [Column("OlusturmaTarihi")]
        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

        [Column("GuncellemeTarihi")]
        public DateTime? GuncellemeTarihi { get; set; }

        [Column("SilindiMi")]
        public bool SilindiMi { get; set; }

        // Navigation
        [ForeignKey(nameof(UstKategoriId))]
        public Kategori? UstKategori { get; set; }

        public ICollection<Kategori> AltKategoriler { get; set; } = new List<Kategori>();
        public ICollection<KategoriAlani> KategoriAlanlari { get; set; } = new List<KategoriAlani>();
        public ICollection<Ilan> Ilanlar { get; set; } = new List<Ilan>();
    }
}
