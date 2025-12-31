
using BusinessLayer.Features.Member.DTOs;
using FluentValidation;
using System.Text.RegularExpressions;

namespace BusinessLayer.Features.Member.Validators
{
    public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileRequestValidator()
        {
            RuleFor(x => x.AdSoyad)
                .NotEmpty().WithMessage("Ad Soyad boş olamaz.")
                .MinimumLength(2).WithMessage("Ad Soyad en az 2 karakter olmalıdır.");

            RuleFor(x => x.PhoneNumber)
                .Must(BeValidPhoneNumber).When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Geçerli bir telefon numarası giriniz.");
        }

        private bool BeValidPhoneNumber(string? phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber)) return true;
            
            // Remove all non-digits
            var digitsOnly = Regex.Replace(phoneNumber, @"\D", "");
            
            // Check length (assuming 10-11 digits for TR/KKTC)
            return digitsOnly.Length >= 10 && digitsOnly.Length <= 11;
        }
    }
}
