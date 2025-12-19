using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EntityLayer.Entities;

namespace DataAccessLayer
{
    public class Context : IdentityDbContext<UygulamaKullanicisi, IdentityRole, string>
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<KategoriAlani> KategoriAlanlari { get; set; }
        public DbSet<KategoriAlaniSecenegi> KategoriAlanSecenekleri { get; set; }

        public DbSet<Ilan> Ilanlar { get; set; }
        public DbSet<IlanFotografi> IlanFotograflari { get; set; }
        public DbSet<IlanAlanDegeri> IlanAlanDegerleri { get; set; }

        public DbSet<Favori> Favoriler { get; set; }
        public DbSet<IlanSorusu> IlanSorulari { get; set; }
        public DbSet<IlanCevabi> IlanCevaplari { get; set; }
        public DbSet<IlanSikayeti> IlanSikayetleri { get; set; }

        public DbSet<Bildirim> Bildirimler { get; set; }
        public DbSet<DenetimKaydi> DenetimKayitlari { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Context).Assembly); 
        }
    }
}
