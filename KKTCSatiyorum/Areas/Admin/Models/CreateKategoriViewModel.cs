namespace KKTCSatiyorum.Areas.Admin.Models
{
    public class CreateKategoriViewModel
    {
        public string Ad { get; set; } = "";
        public string? SeoSlug { get; set; } = "";
        public int? UstKategoriId { get; set; }
        public int SiraNo { get; set; }
    }
}
