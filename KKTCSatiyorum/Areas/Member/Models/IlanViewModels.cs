using BusinessLayer.Features.KategoriAlanlari.DTOs;
using EntityLayer.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KKTCSatiyorum.Areas.Member.Models
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

        public List<AttributeInputModel> Attributes { get; set; } = new();

        public IReadOnlyList<SelectListItem> KategoriOptions { get; set; } = Array.Empty<SelectListItem>();
        public IReadOnlyList<SelectListItem> ParaBirimiOptions { get; set; } = Array.Empty<SelectListItem>();
    }

    public class AttributeInputModel
    {
        public int KategoriAlaniId { get; set; }
        public string? Value { get; set; }
    }

    public class KategoriAttributesDto
    {
        public int Id { get; set; }
        public string Ad { get; set; } = "";
        public string Anahtar { get; set; } = "";
        public VeriTipi VeriTipi { get; set; }
        public bool ZorunluMu { get; set; }
        public List<SecenekDto> Secenekler { get; set; } = new();
    }

    public class SecenekDto
    {
        public int Id { get; set; }
        public string Deger { get; set; } = "";
    }
}
