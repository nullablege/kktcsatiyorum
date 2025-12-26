using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KKTCSatiyorum.Areas.Admin.Models
{
    public class CreateKategoriViewModel
    {
        [Display(Name = "Kategori Adı")]
        public string Ad { get; set; } = "";

        [Display(Name = "Üst Kategori")]
        public int? UstKategoriId { get; set; }

        [Display(Name = "Sıra No")]
        public int SiraNo { get; set; } = 1;

        public IReadOnlyList<SelectListItem> UstKategoriOptions { get; set; } = Array.Empty<SelectListItem>();
    }
}
