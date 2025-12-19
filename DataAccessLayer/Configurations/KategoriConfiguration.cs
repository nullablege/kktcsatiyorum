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
    public class KategoriConfiguration : IEntityTypeConfiguration<Kategori>
    {
        public void Configure(EntityTypeBuilder<Kategori> b)
        {
            b.ToTable("Kategoriler");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).HasColumnName("Id");
            b.Property(x => x.UstKategoriId).HasColumnName("UstKategoriId");
            b.Property(x => x.Ad).HasColumnName("Ad").HasMaxLength(150).IsRequired();
            b.Property(x => x.SeoSlug).HasColumnName("SeoSlug").HasMaxLength(200).IsRequired();
            b.Property(x => x.AktifMi).HasColumnName("AktifMi");
            b.Property(x => x.SiraNo).HasColumnName("SiraNo");
            b.Property(x => x.OlusturmaTarihi).HasColumnName("OlusturmaTarihi").IsRequired();
            b.Property(x => x.GuncellemeTarihi).HasColumnName("GuncellemeTarihi");
            b.Property(x => x.SilindiMi).HasColumnName("SilindiMi");

            b.HasIndex(x => x.SeoSlug).IsUnique();
            b.HasIndex(x => x.UstKategoriId);

            b.HasOne(x => x.UstKategori)
                .WithMany(x => x.AltKategoriler)
                .HasForeignKey(x => x.UstKategoriId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
