using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    [Table("KategoriAlanSecenekleri")]
    public class KategoriAlaniSecenegi
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("KategoriAlaniId")]
        public int KategoriAlaniId { get; set; }

        [Column("Deger")]
        [MaxLength(200)]
        public string Deger { get; set; } = default!;

        [Column("SiraNo")]
        public int SiraNo { get; set; }

        [Column("AktifMi")]
        public bool AktifMi { get; set; } = true;

        // (Opsiyonel) Marka->Model gibi bagimlilik istersen:
        // [Column("UstSecenekId")]
        // public int? UstSecenekId { get; set; }

        [ForeignKey(nameof(KategoriAlaniId))]
        public KategoriAlani KategoriAlani { get; set; } = default!;

        public ICollection<IlanAlanDegeri> IlanAlanDegerleri { get; set; } = new List<IlanAlanDegeri>();
    }
}
