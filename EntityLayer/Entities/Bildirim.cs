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
    [Table("Bildirimler")]
    public class Bildirim
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("KullaniciId")]
        public string KullaniciId { get; set; } = default!;

        [Column("Tur")]
        public BildirimTuru Tur { get; set; }

        [Column("VeriJson")]
        public string? VeriJson { get; set; }

        [Column("Mesaj")]
        public string Mesaj { get; set; } = "";

        [Column("OkunduMu")]
        public bool OkunduMu { get; set; }

        [Column("OlusturmaTarihi")]
        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(KullaniciId))]
        public UygulamaKullanicisi Kullanici { get; set; } = default!;
    }
}
