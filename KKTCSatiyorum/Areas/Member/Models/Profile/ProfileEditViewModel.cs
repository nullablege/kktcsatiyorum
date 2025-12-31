
using System.ComponentModel.DataAnnotations;

namespace KKTCSatiyorum.Areas.Member.Models.Profile
{
    public class ProfileEditViewModel
    {
        [Required(ErrorMessage = "Ad Soyad gereklidir.")]
        [MinLength(2, ErrorMessage = "Ad Soyad en az 2 karakter olmalıdır.")]
        [Display(Name = "Ad Soyad")]
        public string AdSoyad { get; set; } = string.Empty;

        [Display(Name = "E-Posta")]
        public string Email { get; set; } = string.Empty; // ReadOnly display

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [Display(Name = "Telefon Numarası")]
        public string? PhoneNumber { get; set; }

        public string? ProfilFotografiYolu { get; set; }
        
        // Success/Error message for UI feedback
        public string? StatusMessage { get; set; }
    }
}
