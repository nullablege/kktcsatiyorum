using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    [Table("DenetimKayitlari")]
    public class DenetimKaydi
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }

        [Column("KullaniciId")]
        public string? KullaniciId { get; set; }

        [Column("Eylem")]
        [MaxLength(200)]
        public string Eylem { get; set; } = default!;

        [Column("VarlikAdi")]
        [MaxLength(100)]
        public string VarlikAdi { get; set; } = default!;

        [Column("VarlikId")]
        [MaxLength(100)]
        public string VarlikId { get; set; } = default!;

        [Column("DetayJson")]
        public string? DetayJson { get; set; }

        [Column("IpAdresi")]
        [MaxLength(50)]
        public string? IpAdresi { get; set; }

        [Column("OlusturmaTarihi")]
        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(KullaniciId))]
        public UygulamaKullanicisi? Kullanici { get; set; }
    }
}
