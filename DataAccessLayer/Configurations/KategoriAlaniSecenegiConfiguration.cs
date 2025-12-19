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
    public class KategoriAlaniSecenegiConfiguration : IEntityTypeConfiguration<KategoriAlaniSecenegi>
    {
        public void Configure(EntityTypeBuilder<KategoriAlaniSecenegi> b)
        {
            b.ToTable("KategoriAlanSecenekleri");
            b.HasKey(x => x.Id);

            b.Property(x => x.KategoriAlaniId).HasColumnName("KategoriAlaniId").IsRequired();
            b.Property(x => x.Deger).HasColumnName("Deger").HasMaxLength(200).IsRequired();
            b.Property(x => x.SiraNo).HasColumnName("SiraNo");
            b.Property(x => x.AktifMi).HasColumnName("AktifMi");

            b.HasOne(x => x.KategoriAlani)
                .WithMany(x => x.Secenekler)
                .HasForeignKey(x => x.KategoriAlaniId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => x.KategoriAlaniId);
        }
    }
}
