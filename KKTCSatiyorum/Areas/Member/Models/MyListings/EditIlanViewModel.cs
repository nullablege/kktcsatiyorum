using EntityLayer.DTOs.Public;

namespace KKTCSatiyorum.Areas.Member.Models.MyListings
{
    public class EditIlanViewModel : CreateIlanViewModel
    {
        public int Id { get; set; }
        public List<PhotoDto> CurrentPhotos { get; set; } = new();
    }
}
