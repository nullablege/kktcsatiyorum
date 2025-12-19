using DataAccessLayer.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    [Table("KategoriAlanlari")]
    public class KategoriAlani
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("KategoriId")]
        public int KategoriId { get; set; }

        [Column("Ad")]
        [MaxLength(150)]
        public string Ad { get; set; } = default!;

        [Column("Anahtar")]
        [MaxLength(100)]
        public string Anahtar { get; set; } = default!;

        [Column("VeriTipi")]
        public VeriTipi VeriTipi { get; set; }

        [Column("ZorunluMu")]
        public bool ZorunluMu { get; set; }

        [Column("FiltrelenebilirMi")]
        public bool FiltrelenebilirMi { get; set; }

        [Column("SiraNo")]
        public int SiraNo { get; set; }

        [Column("AktifMi")]
        public bool AktifMi { get; set; } = true;

        // Navigation
        [ForeignKey(nameof(KategoriId))]
        public Kategori Kategori { get; set; } = default!;

        public ICollection<KategoriAlaniSecenegi> Secenekler { get; set; } = new List<KategoriAlaniSecenegi>();
        public ICollection<IlanAlanDegeri> IlanAlanDegerleri { get; set; } = new List<IlanAlanDegeri>();
    }
}
