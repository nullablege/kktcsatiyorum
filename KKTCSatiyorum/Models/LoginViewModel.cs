using System.ComponentModel.DataAnnotations;

namespace KKTCSatiyorum.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Þifre gereklidir")]
        [DataType(DataType.Password)]
        [Display(Name = "Þifre")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Beni Hatýrla")]
        public bool RememberMe { get; set; }
    }
}
