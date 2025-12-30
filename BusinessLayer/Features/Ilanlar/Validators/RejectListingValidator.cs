using BusinessLayer.Features.Ilanlar.DTOs;
using FluentValidation;

namespace BusinessLayer.Features.Ilanlar.Validators
{
    public class RejectListingValidator : AbstractValidator<RejectListingRequest>
    {
        public RejectListingValidator()
        {
            RuleFor(x => x.ListingId)
                .GreaterThan(0).WithMessage("Gecersiz ilan ID.");

            RuleFor(x => x.AdminUserId)
                .NotEmpty().WithMessage("Admin kullanici ID bos olamaz.");

            RuleFor(x => x.RedNedeni)
                .NotEmpty().WithMessage("Red nedeni bos olamaz.")
                .MaximumLength(200).WithMessage("Red nedeni en fazla 200 karakter olabilir.");
        }
    }
}
