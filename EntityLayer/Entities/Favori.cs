using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    [Table("Favoriler")]
    public class Favori
    {
        [Column("KullaniciId")]
        public string KullaniciId { get; set; } = default!;

        [Column("IlanId")]
        public int IlanId { get; set; }

        [Column("OlusturmaTarihi")]
        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(KullaniciId))]
        public UygulamaKullanicisi Kullanici { get; set; } = default!;

        [ForeignKey(nameof(IlanId))]
        public Ilan Ilan { get; set; } = default!;
    }
}
