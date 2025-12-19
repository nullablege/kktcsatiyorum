using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations
{
    public class IlanConfiguration : IEntityTypeConfiguration<Ilan>
    {
        public void Configure(EntityTypeBuilder<Ilan> b)
        {
            b.ToTable("Ilanlar");
            b.HasKey(x => x.Id);

            b.Property(x => x.SahipKullaniciId).HasColumnName("SahipKullaniciId").IsRequired();
            b.Property(x => x.KategoriId).HasColumnName("KategoriId").IsRequired();

            b.Property(x => x.Baslik).HasColumnName("Baslik").HasMaxLength(200).IsRequired();
            b.Property(x => x.SeoSlug).HasColumnName("SeoSlug").HasMaxLength(250).IsRequired();
            b.Property(x => x.Aciklama).HasColumnName("Aciklama").IsRequired();

            b.Property(x => x.Fiyat).HasColumnName("Fiyat").HasPrecision(18, 2).IsRequired();
            b.Property(x => x.ParaBirimi).HasColumnName("ParaBirimi").HasConversion<short>().IsRequired();
            b.Property(x => x.Durum).HasColumnName("Durum").HasConversion<short>().IsRequired();

            b.Property(x => x.RedNedeni).HasColumnName("RedNedeni").HasMaxLength(500);
            b.Property(x => x.Sehir).HasColumnName("Sehir").HasMaxLength(100);
            b.Property(x => x.Ilce).HasColumnName("Ilce").HasMaxLength(120);

            b.Property(x => x.Enlem).HasColumnName("Enlem").HasPrecision(9, 6);
            b.Property(x => x.Boylam).HasColumnName("Boylam").HasPrecision(9, 6);

            b.Property(x => x.GoruntulenmeSayisi).HasColumnName("GoruntulenmeSayisi");
            b.Property(x => x.OlusturmaTarihi).HasColumnName("OlusturmaTarihi").IsRequired();
            b.Property(x => x.GuncellemeTarihi).HasColumnName("GuncellemeTarihi");
            b.Property(x => x.YayinTarihi).HasColumnName("YayinTarihi");
            b.Property(x => x.OnaylayanKullaniciId).HasColumnName("OnaylayanKullaniciId");
            b.Property(x => x.OnayTarihi).HasColumnName("OnayTarihi");
            b.Property(x => x.SilindiMi).HasColumnName("SilindiMi");

            b.HasOne(x => x.Kategori)
                .WithMany(x => x.Ilanlar)
                .HasForeignKey(x => x.KategoriId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.SahipKullanici)
                .WithMany(x => x.Ilanlar)
                .HasForeignKey(x => x.SahipKullaniciId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.OnaylayanKullanici)
                .WithMany()
                .HasForeignKey(x => x.OnaylayanKullaniciId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => new { x.KategoriId, x.Durum, x.OlusturmaTarihi });
            b.HasIndex(x => x.SahipKullaniciId);
            b.HasIndex(x => x.Fiyat);
        }
    }
}
