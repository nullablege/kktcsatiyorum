using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KKTCSatiyorum.Areas.Admin.Models
{
    public class UpdateKategoriViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Kategori Adı")]
        public string Ad { get; set; } = "";

        [Display(Name = "Üst Kategori")]
        public int? UstKategoriId { get; set; }

        [Display(Name = "Sıra No")]
        public int SiraNo { get; set; }

        [Display(Name = "Aktif mi?")]
        public bool AktifMi { get; set; }

        public IReadOnlyList<SelectListItem> UstKategoriOptions { get; set; } = Array.Empty<SelectListItem>();
    }
}
