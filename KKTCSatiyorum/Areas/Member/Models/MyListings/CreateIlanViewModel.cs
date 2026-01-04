using BusinessLayer.Features.KategoriAlanlari.DTOs;
using EntityLayer.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KKTCSatiyorum.Areas.Member.Models.MyListings
{
    public class CreateIlanViewModel
    {
        [Display(Name = "Kategori")]
        public int KategoriId { get; set; }

        [Display(Name = "Başlık")]
        public string Baslik { get; set; } = "";

        [Display(Name = "Açıklama")]
        public string Aciklama { get; set; } = "";

        [Display(Name = "Fiyat")]
        public decimal Fiyat { get; set; }

        [Display(Name = "Para Birimi")]
        public ParaBirimi ParaBirimi { get; set; } = ParaBirimi.TRY;

        [Display(Name = "Şehir")]
        public string Sehir { get; set; } = "";

        [Display(Name = "İlçe")]
        [StringLength(120, ErrorMessage = "İlçe en fazla 120 karakter olabilir.")]
        public string? Ilce { get; set; }

        [Display(Name = "Enlem")]
        [Range(-90, 90, ErrorMessage = "Enlem -90 ile 90 arasında olmalıdır.")]
        public decimal? Enlem { get; set; }

        [Display(Name = "Boylam")]
        [Range(-180, 180, ErrorMessage = "Boylam -180 ile 180 arasında olmalıdır.")]
        public decimal? Boylam { get; set; }

        public List<AttributeInputModel> Attributes { get; set; } = new();

        public IReadOnlyList<SelectListItem> KategoriOptions { get; set; } = Array.Empty<SelectListItem>();
        public IReadOnlyList<SelectListItem> ParaBirimiOptions { get; set; } = Array.Empty<SelectListItem>();
    }
}
