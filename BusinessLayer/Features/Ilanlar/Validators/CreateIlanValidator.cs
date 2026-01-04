using BusinessLayer.Features.Ilanlar.DTOs;
using FluentValidation;

namespace BusinessLayer.Features.Ilanlar.Validators
{
    public class CreateIlanValidator : AbstractValidator<CreateIlanRequest>
    {
        public CreateIlanValidator()
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
                .InclusiveBetween(-90, 90).When(x => x.Enlem.HasValue).WithMessage("Geçerli bir enlem değeri giriniz.");

            RuleFor(x => x.Boylam)
                .InclusiveBetween(-180, 180).When(x => x.Boylam.HasValue).WithMessage("Geçerli bir boylam değeri giriniz.");

            RuleFor(x => x.PhotoPaths)
                .NotEmpty().WithMessage("En az bir fotoğraf yükleyiniz.");
        }
    }
}
