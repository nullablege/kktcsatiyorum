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
    public class IlanFotografiConfiguration : IEntityTypeConfiguration<IlanFotografi>
    {
        public void Configure(EntityTypeBuilder<IlanFotografi> b)
        {
            b.ToTable("IlanFotograflari");
            b.HasKey(x => x.Id);

            b.Property(x => x.IlanId).HasColumnName("IlanId").IsRequired();
            b.Property(x => x.DosyaYolu).HasColumnName("DosyaYolu").HasMaxLength(400).IsRequired();
            b.Property(x => x.KapakMi).HasColumnName("KapakMi");
            b.Property(x => x.SiraNo).HasColumnName("SiraNo");
            b.Property(x => x.OlusturmaTarihi).HasColumnName("OlusturmaTarihi").IsRequired();

            b.HasOne(x => x.Ilan)
                .WithMany(x => x.Fotografler)
                .HasForeignKey(x => x.IlanId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => x.IlanId);
        }
    }
}
