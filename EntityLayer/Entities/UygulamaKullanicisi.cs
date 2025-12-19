using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class UygulamaKullanicisi : IdentityUser<string>
    {
        public UygulamaKullanicisi()
        {
            Id = Guid.NewGuid().ToString();
        }

        [Column("AdSoyad")]
        [MaxLength(150)]
        public string AdSoyad { get; set; } = null!;

        [Column("ProfilFotografYolu")]
        [MaxLength(400)]
        public string? ProfilFotografYolu { get; set; }

        [Column("AskidaMi")]
        public bool AskidaMi { get; set; } = false;

        [Column("AskidaBitisTarihi")]
        public DateTime? AskidaBitisTarihi { get; set; }

        [Column("OlusturmaTarihi")]
        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

        [Column("SonGirisTarihi")]
        public DateTime? SonGirisTarihi { get; set; }

        // Navigation
        public ICollection<Ilan> Ilanlar { get; set; } = new List<Ilan>();
        public ICollection<Favori> Favoriler { get; set; } = new List<Favori>();
        public ICollection<Bildirim> Bildirimler { get; set; } = new List<Bildirim>();
        public ICollection<DenetimKaydi> DenetimKayitlari { get; set; } = new List<DenetimKaydi>();
    }
}
