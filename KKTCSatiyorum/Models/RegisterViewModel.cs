using System.ComponentModel.DataAnnotations;

namespace KKTCSatiyorum.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad gereklidir")]
        [StringLength(75)]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad gereklidir")]
        [StringLength(75)]
        public string Soyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir email giriniz")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Geçerli bir telefon numarasý giriniz")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Þifre gereklidir")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Þifre tekrarý gereklidir")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Þifreler eþleþmiyor")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
