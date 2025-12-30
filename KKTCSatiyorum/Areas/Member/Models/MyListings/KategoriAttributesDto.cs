using EntityLayer.Enums;

namespace KKTCSatiyorum.Areas.Member.Models.MyListings
{
    public class KategoriAttributesDto
    {
        public int Id { get; set; }
        public string Ad { get; set; } = "";
        public string Anahtar { get; set; } = "";
        public VeriTipi VeriTipi { get; set; }
        public bool ZorunluMu { get; set; }
        public List<SecenekDto> Secenekler { get; set; } = new();
    }
}
