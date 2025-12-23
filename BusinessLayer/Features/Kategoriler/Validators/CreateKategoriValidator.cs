using BusinessLayer.Features.Kategoriler.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Features.Kategoriler.Validators
{
    public sealed class CreateKategoriValidator : AbstractValidator<CreateKategoriRequest>
    {
        public CreateKategoriValidator()
        {
            RuleFor(x => x.Ad)
                .NotEmpty().WithMessage("Ad boş olamaz.")
                .MinimumLength(2).WithMessage("Ad en az 2 karakter olmalı.")
                .MaximumLength(100).WithMessage("Ad en fazla 100 karakter olmalı.");



            RuleFor(x => x.SiraNo)
                .GreaterThanOrEqualTo(0).WithMessage("SiraNo 0 veya daha büyük olmalı.");
        }
    }
}
