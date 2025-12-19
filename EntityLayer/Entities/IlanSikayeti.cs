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
    [Table("IlanSikayetleri")]
    public class IlanSikayeti
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("IlanId")]
        public int IlanId { get; set; }

        [Column("SikayetEdenKullaniciId")]
        public string SikayetEdenKullaniciId { get; set; } = default!;

        [Column("Neden")]
        public SikayetNedeni Neden { get; set; }

        [Column("Aciklama")]
        public string? Aciklama { get; set; }

        [Column("Durum")]
        public SikayetDurumu Durum { get; set; } = SikayetDurumu.Acik;

        [Column("OlusturmaTarihi")]
        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(IlanId))]
        public Ilan Ilan { get; set; } = default!;

        [ForeignKey(nameof(SikayetEdenKullaniciId))]
        public UygulamaKullanicisi SikayetEdenKullanici { get; set; } = default!;
    }
}
