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
    public class IlanCevabiConfiguration : IEntityTypeConfiguration<IlanCevabi>
    {
        public void Configure(EntityTypeBuilder<IlanCevabi> b)
        {
            b.ToTable("IlanCevaplari");
            b.HasKey(x => x.Id);

            b.Property(x => x.SoruId).HasColumnName("SoruId").IsRequired();
            b.Property(x => x.CevaplayanKullaniciId).HasColumnName("CevaplayanKullaniciId").IsRequired();
            b.Property(x => x.CevapMetni).HasColumnName("CevapMetni").IsRequired();
            b.Property(x => x.OlusturmaTarihi).HasColumnName("OlusturmaTarihi").IsRequired();

            b.HasIndex(x => x.SoruId).IsUnique(); // 1 soru 1 cevap

            b.HasOne(x => x.Soru).WithOne(x => x.Cevap).HasForeignKey<IlanCevabi>(x => x.SoruId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.CevaplayanKullanici).WithMany().HasForeignKey(x => x.CevaplayanKullaniciId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
