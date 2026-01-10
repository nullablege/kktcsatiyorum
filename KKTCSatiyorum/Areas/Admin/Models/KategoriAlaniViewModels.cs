using BusinessLayer.Features.KategoriAlanlari.DTOs;
using EntityLayer.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KKTCSatiyorum.Areas.Admin.Models
{
    public class KategoriAlaniIndexViewModel
    {
        public int? SelectedKategoriId { get; set; }
        public string? KategoriAdi { get; set; }
        public IReadOnlyList<SelectListItem> KategoriOptions { get; set; } = Array.Empty<SelectListItem>();
        public IReadOnlyList<KategoriAlaniListItemDto> Items { get; set; } = Array.Empty<KategoriAlaniListItemDto>();
    }

    public class CreateKategoriAlaniViewModel
    {
        public int KategoriId { get; set; }
        public string? KategoriAdi { get; set; }

        [Display(Name = "Alan Adı")]
        public string Ad { get; set; } = "";

        [Display(Name = "Anahtar")]
        public string Anahtar { get; set; } = "";

        [Display(Name = "Veri Tipi")]
        public VeriTipi VeriTipi { get; set; } = VeriTipi.Metin;

        [Display(Name = "Zorunlu mu?")]
        public bool ZorunluMu { get; set; }

        [Display(Name = "Filtrelenebilir mi?")]
        public bool FiltrelenebilirMi { get; set; }

        [Display(Name = "Sıra No")]
        public int SiraNo { get; set; } = 1;

        [Display(Name = "Seçenekler (virgülle ayırın)")]
        public string? SeceneklerText { get; set; }

        public IReadOnlyList<SelectListItem> VeriTipiOptions { get; set; } = Array.Empty<SelectListItem>();
    }

    public class EditKategoriAlaniViewModel
    {
        public int Id { get; set; }
        public int KategoriId { get; set; }
        public string? KategoriAdi { get; set; }

        [Display(Name = "Alan Adı")]
        public string Ad { get; set; } = "";

        [Display(Name = "Anahtar")]
        public string Anahtar { get; set; } = "";

        [Display(Name = "Veri Tipi")]
        public VeriTipi VeriTipi { get; set; }

        [Display(Name = "Zorunlu mu?")]
        public bool ZorunluMu { get; set; }

        [Display(Name = "Filtrelenebilir mi?")]
        public bool FiltrelenebilirMi { get; set; }

        [Display(Name = "Sıra No")]
        public int SiraNo { get; set; }

        [Display(Name = "Aktif mi?")]
        public bool AktifMi { get; set; }

        public IReadOnlyList<SelectListItem> VeriTipiOptions { get; set; } = Array.Empty<SelectListItem>();
        public IReadOnlyList<KategoriAlaniSecenegiDto> Secenekler { get; set; } = Array.Empty<KategoriAlaniSecenegiDto>();
        
        [Display(Name = "Seçenekler (virgülle ayırın)")]
        public string? SeceneklerText { get; set; }
    }
}
