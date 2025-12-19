using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    [Table("IlanFotograflari")]
    public class IlanFotografi
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("IlanId")]
        public int IlanId { get; set; }

        [Column("DosyaYolu")]
        [MaxLength(400)]
        public string DosyaYolu { get; set; } = default!;

        [Column("KapakMi")]
        public bool KapakMi { get; set; }

        [Column("SiraNo")]
        public int SiraNo { get; set; }

        [Column("OlusturmaTarihi")]
        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(IlanId))]
        public Ilan Ilan { get; set; } = default!;
    }
}
