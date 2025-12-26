using BusinessLayer.Features.Kategoriler.DTOs;

namespace KKTCSatiyorum.Areas.Admin.Models
{
    public class KategoriIndexViewModel
    {
        public IReadOnlyList<KategoriListItemDto> Items { get; set; } = Array.Empty<KategoriListItemDto>();
        public UpdateKategoriViewModel? Selected { get; set; }
    }
}
