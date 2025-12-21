using EntityLayer.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    [Table("IlanSorulari")]
    public class IlanSorusu
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("IlanId")]
        public int IlanId { get; set; }

        [Column("SoranKullaniciId")]
        public string SoranKullaniciId { get; set; } = default!;

        [Column("SoruMetni")]
        public string SoruMetni { get; set; } = default!;

        [Column("Durum")]
        public SoruDurumu Durum { get; set; } = SoruDurumu.Aktif;

        [Column("OlusturmaTarihi")]
        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(IlanId))]
        public Ilan Ilan { get; set; } = default!;

        [ForeignKey(nameof(SoranKullaniciId))]
        public UygulamaKullanicisi SoranKullanici { get; set; } = default!;

        public IlanCevabi? Cevap { get; set; }
    }
}
