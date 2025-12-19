using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    [Table("IlanCevaplari")]
    public class IlanCevabi
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("SoruId")]
        public int SoruId { get; set; }

        [Column("CevaplayanKullaniciId")]
        public string CevaplayanKullaniciId { get; set; } = default!;

        [Column("CevapMetni")]
        public string CevapMetni { get; set; } = default!;

        [Column("OlusturmaTarihi")]
        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(SoruId))]
        public IlanSorusu Soru { get; set; } = default!;

        [ForeignKey(nameof(CevaplayanKullaniciId))]
        public UygulamaKullanicisi CevaplayanKullanici { get; set; } = default!;
    }
}
