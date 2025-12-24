using BusinessLayer.Features.Kategoriler.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Features.Kategoriler.Validators
{
    public sealed class KategoriSoftDeleteValidator:AbstractValidator<SoftDeleteKategoriRequest>
    {
        public KategoriSoftDeleteValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("ID Boş olamaz.")
                .GreaterThan(0).WithMessage("ID'nin 0 dan büyük olması gerekiyor.");
        }
    }
}
