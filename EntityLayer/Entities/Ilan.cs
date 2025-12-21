using EntityLayer.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 

namespace EntityLayer.Entities
{
    [Table("Ilanlar")]
    public class Ilan
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("SahipKullaniciId")]
        public string SahipKullaniciId { get; set; } = default!;

        [Column("KategoriId")]
        public int KategoriId { get; set; }

        [Column("Baslik")]
        [MaxLength(200)]
        public string Baslik { get; set; } = default!;

        [Column("SeoSlug")]
        [MaxLength(250)]
        public string SeoSlug { get; set; } = default!;

        [Column("Aciklama")]
        public string Aciklama { get; set; } = default!;

        [Column("Fiyat")]
        [Precision(18, 2)]
        public decimal Fiyat { get; set; }

        [Column("ParaBirimi")]
        public ParaBirimi ParaBirimi { get; set; } = ParaBirimi.TRY;

        [Column("Durum")]
        public IlanDurumu Durum { get; set; } = IlanDurumu.Taslak;

        [Column("RedNedeni")]
        [MaxLength(500)]
        public string? RedNedeni { get; set; }

        [Column("Sehir")]
        [MaxLength(100)]
        public string? Sehir { get; set; }

        [Column("Ilce")]
        [MaxLength(120)]
        public string? Ilce { get; set; }

        [Column("Enlem")]
        [Precision(9, 6)]
        public decimal? Enlem { get; set; }

        [Column("Boylam")]
        [Precision(9, 6)]
        public decimal? Boylam { get; set; }

        [Column("GoruntulenmeSayisi")]
        public int GoruntulenmeSayisi { get; set; }

        [Column("OlusturmaTarihi")]
        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

        [Column("GuncellemeTarihi")]
        public DateTime? GuncellemeTarihi { get; set; }

        [Column("YayinTarihi")]
        public DateTime? YayinTarihi { get; set; }

        [Column("OnaylayanKullaniciId")]
        public string? OnaylayanKullaniciId { get; set; }

        [Column("OnayTarihi")]
        public DateTime? OnayTarihi { get; set; }

        [Column("SilindiMi")]
        public bool SilindiMi { get; set; }

        // Navigation
        [ForeignKey(nameof(SahipKullaniciId))]
        public UygulamaKullanicisi SahipKullanici { get; set; } = default!;

        [ForeignKey(nameof(KategoriId))]
        public Kategori Kategori { get; set; } = default!;

        [ForeignKey(nameof(OnaylayanKullaniciId))]
        public UygulamaKullanicisi? OnaylayanKullanici { get; set; }

        public ICollection<IlanFotografi> Fotografler { get; set; } = new List<IlanFotografi>();
        public ICollection<IlanAlanDegeri> AlanDegerleri { get; set; } = new List<IlanAlanDegeri>();
        public ICollection<IlanSorusu> Sorular { get; set; } = new List<IlanSorusu>();
        public ICollection<IlanSikayeti> Sikayetler { get; set; } = new List<IlanSikayeti>();
        public ICollection<Favori> Favoriler { get; set; } = new List<Favori>();
    }
}
