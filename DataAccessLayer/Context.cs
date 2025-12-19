using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class Context : IdentityDbContext<UygulamaKullanicisi>
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
