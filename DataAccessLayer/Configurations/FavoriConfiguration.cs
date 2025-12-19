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
    public class FavoriConfiguration : IEntityTypeConfiguration<Favori>
    {
        public void Configure(EntityTypeBuilder<Favori> b)
        {
            b.ToTable("Favoriler");
            b.HasKey(x => new { x.KullaniciId, x.IlanId });

            b.Property(x => x.KullaniciId).HasColumnName("KullaniciId").IsRequired();
            b.Property(x => x.IlanId).HasColumnName("IlanId").IsRequired();
            b.Property(x => x.OlusturmaTarihi).HasColumnName("OlusturmaTarihi").IsRequired();

            b.HasOne(x => x.Kullanici)
                .WithMany(x => x.Favoriler)
                .HasForeignKey(x => x.KullaniciId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Ilan)
                .WithMany(x => x.Favoriler)
                .HasForeignKey(x => x.IlanId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => x.IlanId);
        }
    }
}
