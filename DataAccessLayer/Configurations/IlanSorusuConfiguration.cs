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
    public class IlanSorusuConfiguration : IEntityTypeConfiguration<IlanSorusu>
    {
        public void Configure(EntityTypeBuilder<IlanSorusu> b)
        {
            b.ToTable("IlanSorulari");
            b.HasKey(x => x.Id);

            b.Property(x => x.IlanId).HasColumnName("IlanId").IsRequired();
            b.Property(x => x.SoranKullaniciId).HasColumnName("SoranKullaniciId").IsRequired();
            b.Property(x => x.SoruMetni).HasColumnName("SoruMetni").IsRequired();
            b.Property(x => x.Durum).HasColumnName("Durum").HasConversion<short>().IsRequired();
            b.Property(x => x.OlusturmaTarihi).HasColumnName("OlusturmaTarihi").IsRequired();

            b.HasOne(x => x.Ilan).WithMany(x => x.Sorular).HasForeignKey(x => x.IlanId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.SoranKullanici).WithMany().HasForeignKey(x => x.SoranKullaniciId).OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => x.IlanId);
        }
    }

   
}
