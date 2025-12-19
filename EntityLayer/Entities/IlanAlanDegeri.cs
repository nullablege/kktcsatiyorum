using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    [Table("IlanAlanDegerleri")]
    public class IlanAlanDegeri
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("IlanId")]
        public int IlanId { get; set; }

        [Column("KategoriAlaniId")]
        public int KategoriAlaniId { get; set; }

        // TekSecim icin
        [Column("SecenekId")]
        public int? SecenekId { get; set; }

        // Diger tipler icin
        [Column("MetinDeger")]
        [MaxLength(500)]
        public string? MetinDeger { get; set; }

        [Column("TamSayiDeger")]
        public long? TamSayiDeger { get; set; }

        [Column("OndalikDeger")]
        [Precision(18, 4)]
        public decimal? OndalikDeger { get; set; }

        [Column("DogruYanlisDeger")]
        public bool? DogruYanlisDeger { get; set; }

        [Column("TarihDeger")]
        public DateTime? TarihDeger { get; set; }

        // Navigation
        [ForeignKey(nameof(IlanId))]
        public Ilan Ilan { get; set; } = default!;

        [ForeignKey(nameof(KategoriAlaniId))]
        public KategoriAlani KategoriAlani { get; set; } = default!;

        [ForeignKey(nameof(SecenekId))]
        public KategoriAlaniSecenegi? Secenek { get; set; }
    }
}
