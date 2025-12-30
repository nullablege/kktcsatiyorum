using BusinessLayer.Features.Ilanlar.DTOs;
using FluentValidation;

namespace BusinessLayer.Features.Ilanlar.Validators
{
    public class ApproveListingValidator : AbstractValidator<ApproveListingRequest>
    {
        public ApproveListingValidator()
        {
            RuleFor(x => x.ListingId)
                .GreaterThan(0).WithMessage("Gecersiz ilan ID.");

            RuleFor(x => x.AdminUserId)
                .NotEmpty().WithMessage("Admin kullanici ID bos olamaz.");
        }
    }
}
