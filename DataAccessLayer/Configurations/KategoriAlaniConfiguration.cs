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
    public class KategoriAlaniConfiguration : IEntityTypeConfiguration<KategoriAlani>
    {
        public void Configure(EntityTypeBuilder<KategoriAlani> b)
        {
            b.ToTable("KategoriAlanlari");
            b.HasKey(x => x.Id);

            b.Property(x => x.KategoriId).HasColumnName("KategoriId").IsRequired();
            b.Property(x => x.Ad).HasColumnName("Ad").HasMaxLength(150).IsRequired();
            b.Property(x => x.Anahtar).HasColumnName("Anahtar").HasMaxLength(100).IsRequired();

            b.Property(x => x.VeriTipi).HasColumnName("VeriTipi").HasConversion<short>().IsRequired();
            b.Property(x => x.ZorunluMu).HasColumnName("ZorunluMu");
            b.Property(x => x.FiltrelenebilirMi).HasColumnName("FiltrelenebilirMi");
            b.Property(x => x.SiraNo).HasColumnName("SiraNo");
            b.Property(x => x.AktifMi).HasColumnName("AktifMi");

            b.HasOne(x => x.Kategori)
                .WithMany(x => x.KategoriAlanlari)
                .HasForeignKey(x => x.KategoriId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => new { x.KategoriId, x.Anahtar }).IsUnique();
            b.HasIndex(x => x.KategoriId);
        }
    }
}
