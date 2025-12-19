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
    public class IlanSikayetiConfiguration : IEntityTypeConfiguration<IlanSikayeti>
    {
        public void Configure(EntityTypeBuilder<IlanSikayeti> b)
        {
            b.ToTable("IlanSikayetleri");
            b.HasKey(x => x.Id);

            b.Property(x => x.IlanId).HasColumnName("IlanId").IsRequired();
            b.Property(x => x.SikayetEdenKullaniciId).HasColumnName("SikayetEdenKullaniciId").IsRequired();
            b.Property(x => x.Neden).HasColumnName("Neden").HasConversion<short>().IsRequired();
            b.Property(x => x.Aciklama).HasColumnName("Aciklama");
            b.Property(x => x.Durum).HasColumnName("Durum").HasConversion<short>().IsRequired();
            b.Property(x => x.OlusturmaTarihi).HasColumnName("OlusturmaTarihi").IsRequired();

            b.HasOne(x => x.Ilan).WithMany(x => x.Sikayetler).HasForeignKey(x => x.IlanId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.SikayetEdenKullanici).WithMany().HasForeignKey(x => x.SikayetEdenKullaniciId).OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => x.IlanId);
        }
    }

 
   
}
