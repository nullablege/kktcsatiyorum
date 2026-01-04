using BusinessLayer.Features.Ilanlar.DTOs;
using FluentValidation;

namespace BusinessLayer.Features.Ilanlar.Validators
{
    public class UpdateIlanValidator : AbstractValidator<UpdateIlanRequest>
    {
        public UpdateIlanValidator()
        {
            RuleFor(x => x.KategoriId)
                .GreaterThan(0).WithMessage("Kategori seçiniz.");

            RuleFor(x => x.Baslik)
                .NotEmpty().WithMessage("Başlık boş olamaz.")
                .MaximumLength(200).WithMessage("Başlık en fazla 200 karakter olabilir.");

            RuleFor(x => x.Aciklama)
                .NotEmpty().WithMessage("Açıklama boş olamaz.")
                .MinimumLength(20).WithMessage("Açıklama en az 20 karakter olmalıdır.");

            RuleFor(x => x.Fiyat)
                .GreaterThanOrEqualTo(0).WithMessage("Fiyat 0 veya daha büyük olmalıdır.");

            RuleFor(x => x.Sehir)
                .NotEmpty().WithMessage("Şehir boş olamaz.")
                .MaximumLength(100).WithMessage("Şehir en fazla 100 karakter olabilir.");

            RuleFor(x => x.Ilce)
                .MaximumLength(120).WithMessage("İlçe en fazla 120 karakter olabilir.");

            RuleFor(x => x.Enlem)
                .InclusiveBetween(-90m, 90m).WithMessage("Enlem -90 ile 90 arasında olmalıdır.")
                .When(x => x.Enlem.HasValue);

            RuleFor(x => x.Boylam)
                .InclusiveBetween(-180m, 180m).WithMessage("Boylam -180 ile 180 arasında olmalıdır.")
                .When(x => x.Boylam.HasValue);
        }
    }
}
