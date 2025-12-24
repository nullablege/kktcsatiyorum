using BusinessLayer.Features.Kategoriler.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Features.Kategoriler.Validators
{
    public sealed class UpdateKategoriValidator : AbstractValidator<UpdateKategoriRequest>
    {
        public UpdateKategoriValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("ID Boş olamaz.")
                .GreaterThan(0).WithMessage("ID Değeri 0'dan büyük olması gerekiyor.");


            RuleFor(x => x.Ad).NotEmpty().WithMessage("Ad Boş olamaz.")
                .MinimumLength(2).WithMessage("Ad 2 karakterden kısa olamaz.")
                .MaximumLength(100).WithMessage("Ad 100 karakterden uzun olamaz.");

            RuleFor(x => x.SiraNo).NotEmpty().WithMessage("SıraNo Boş olamaz.")
                .GreaterThan(0).WithMessage("SıraNo'nun 0'dan büyük olması gerekiyor.");

        }
    }
}
