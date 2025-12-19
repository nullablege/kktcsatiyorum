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
    public class BildirimConfiguration : IEntityTypeConfiguration<Bildirim>
    {
        public void Configure(EntityTypeBuilder<Bildirim> b)
        {
            b.ToTable("Bildirimler");
            b.HasKey(x => x.Id);

            b.Property(x => x.KullaniciId).HasColumnName("KullaniciId").IsRequired();
            b.Property(x => x.Tur).HasColumnName("Tur").HasConversion<short>().IsRequired();
            b.Property(x => x.VeriJson).HasColumnName("VeriJson");
            b.Property(x => x.OkunduMu).HasColumnName("OkunduMu");
            b.Property(x => x.OlusturmaTarihi).HasColumnName("OlusturmaTarihi").IsRequired();

            b.HasOne(x => x.Kullanici).WithMany(x => x.Bildirimler).HasForeignKey(x => x.KullaniciId).OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => new { x.KullaniciId, x.OkunduMu });
        }
    }

}
