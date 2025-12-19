using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations
{
    public class DenetimKaydiConfiguration : IEntityTypeConfiguration<DenetimKaydi>
    {
        public void Configure(EntityTypeBuilder<DenetimKaydi> b)
        {
            b.ToTable("DenetimKayitlari");
            b.HasKey(x => x.Id);

            b.Property(x => x.KullaniciId).HasColumnName("KullaniciId");
            b.Property(x => x.Eylem).HasColumnName("Eylem").HasMaxLength(200).IsRequired();
            b.Property(x => x.VarlikAdi).HasColumnName("VarlikAdi").HasMaxLength(100).IsRequired();
            b.Property(x => x.VarlikId).HasColumnName("VarlikId").HasMaxLength(100).IsRequired();
            b.Property(x => x.DetayJson).HasColumnName("DetayJson");
            b.Property(x => x.IpAdresi).HasColumnName("IpAdresi").HasMaxLength(50);
            b.Property(x => x.OlusturmaTarihi).HasColumnName("OlusturmaTarihi").IsRequired();

            b.HasOne(x => x.Kullanici).WithMany(x => x.DenetimKayitlari).HasForeignKey(x => x.KullaniciId).OnDelete(DeleteBehavior.SetNull);

            b.HasIndex(x => x.OlusturmaTarihi);
        }
    }
}
