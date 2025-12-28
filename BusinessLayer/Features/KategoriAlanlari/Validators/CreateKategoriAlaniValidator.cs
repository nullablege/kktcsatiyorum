using BusinessLayer.Features.KategoriAlanlari.DTOs;
using EntityLayer.Enums;
using FluentValidation;

namespace BusinessLayer.Features.KategoriAlanlari.Validators
{
    public sealed class CreateKategoriAlaniValidator : AbstractValidator<CreateKategoriAlaniRequest>
    {
        public CreateKategoriAlaniValidator()
        {
            RuleFor(x => x.KategoriId)
                .GreaterThan(0).WithMessage("Kategori seçimi zorunludur.");

            RuleFor(x => x.Ad)
                .NotEmpty().WithMessage("Ad boş olamaz.")
                .MinimumLength(2).WithMessage("Ad en az 2 karakter olmalı.")
                .MaximumLength(150).WithMessage("Ad en fazla 150 karakter olmalı.");

            RuleFor(x => x.Anahtar)
                .NotEmpty().WithMessage("Anahtar boş olamaz.")
                .MinimumLength(2).WithMessage("Anahtar en az 2 karakter olmalı.")
                .MaximumLength(100).WithMessage("Anahtar en fazla 100 karakter olmalı.")
                .Matches("^[a-z_][a-z0-9_]*$").WithMessage("Anahtar sadece küçük harf, rakam ve alt çizgi içerebilir.");

            RuleFor(x => x.VeriTipi)
                .IsInEnum().WithMessage("Geçerli bir veri tipi seçiniz.");

            RuleFor(x => x.SiraNo)
                .GreaterThanOrEqualTo(0).WithMessage("Sıra numarası negatif olamaz.");

            RuleFor(x => x.Secenekler)
                .NotEmpty().WithMessage("Tek Seçim veri tipi için en az bir seçenek gereklidir.")
                .When(x => x.VeriTipi == VeriTipi.TekSecim);
        }
    }
}
