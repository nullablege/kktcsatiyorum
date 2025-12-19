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
    public class IlanAlanDegeriConfiguration : IEntityTypeConfiguration<IlanAlanDegeri>
    {
        public void Configure(EntityTypeBuilder<IlanAlanDegeri> b)
        {
            b.ToTable("IlanAlanDegerleri");
            b.HasKey(x => x.Id);

            b.Property(x => x.IlanId).HasColumnName("IlanId").IsRequired();
            b.Property(x => x.KategoriAlaniId).HasColumnName("KategoriAlaniId").IsRequired();
            b.Property(x => x.SecenekId).HasColumnName("SecenekId");

            b.Property(x => x.MetinDeger).HasColumnName("MetinDeger").HasMaxLength(500);
            b.Property(x => x.TamSayiDeger).HasColumnName("TamSayiDeger");
            b.Property(x => x.OndalikDeger).HasColumnName("OndalikDeger").HasPrecision(18, 4);
            b.Property(x => x.DogruYanlisDeger).HasColumnName("DogruYanlisDeger");
            b.Property(x => x.TarihDeger).HasColumnName("TarihDeger");

            b.HasOne(x => x.Ilan)
                .WithMany(x => x.AlanDegerleri)
                .HasForeignKey(x => x.IlanId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.KategoriAlani)
                .WithMany(x => x.IlanAlanDegerleri)
                .HasForeignKey(x => x.KategoriAlaniId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Secenek)
                .WithMany(x => x.IlanAlanDegerleri)
                .HasForeignKey(x => x.SecenekId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => new { x.IlanId, x.KategoriAlaniId }).IsUnique();

            b.HasIndex(x => new { x.KategoriAlaniId, x.SecenekId });
            b.HasIndex(x => new { x.KategoriAlaniId, x.OndalikDeger });
            b.HasIndex(x => new { x.KategoriAlaniId, x.TamSayiDeger });
        }
    }
}
