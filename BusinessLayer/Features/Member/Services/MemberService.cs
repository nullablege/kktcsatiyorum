using BusinessLayer.Common.Constants;
using BusinessLayer.Common.Results;
using BusinessLayer.Features.Member.DTOs;
using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using EntityLayer.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace BusinessLayer.Features.Member.Services
{
    public class MemberService : IMemberService
    {
        private readonly IIlanDal _ilanDal;
        private readonly IFavoriDal _favoriDal;
        private readonly IBildirimDal _bildirimDal;
        private readonly UserManager<UygulamaKullanicisi> _userManager;
        private readonly IValidator<UpdateProfileRequest> _profileValidator;

        public MemberService(
            IIlanDal ilanDal,
            IFavoriDal favoriDal,
            IBildirimDal bildirimDal,
            UserManager<UygulamaKullanicisi> userManager,
            IValidator<UpdateProfileRequest> profileValidator)
        {
            _ilanDal = ilanDal;
            _favoriDal = favoriDal;
            _bildirimDal = bildirimDal;
            _userManager = userManager;
            _profileValidator = profileValidator;
        }

        public async Task<Result<MemberDashboardStatsDto>> GetDashboardStatsAsync(string userId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Result<MemberDashboardStatsDto>.Fail(ErrorType.Validation, ErrorCodes.Member.UserNotFound, "Kullanıcı ID boş olamaz.");
            }

            var activeCount = await _ilanDal.CountAsync(x => x.SahipKullaniciId == userId && x.Durum == IlanDurumu.Yayinda && !x.SilindiMi, ct);
            var pendingCount = await _ilanDal.CountAsync(x => x.SahipKullaniciId == userId && x.Durum == IlanDurumu.OnayBekliyor && !x.SilindiMi, ct);
            var favoriteCount = await _favoriDal.CountAsync(x => x.KullaniciId == userId, ct);
            var notificationCount = await _bildirimDal.CountAsync(x => x.KullaniciId == userId && !x.OkunduMu, ct);

            var stats = new MemberDashboardStatsDto
            {
                ActiveListingCount = activeCount,
                PendingListingCount = pendingCount,
                FavoriteCount = favoriteCount,
                UnreadNotificationCount = notificationCount
            };

            return Result<MemberDashboardStatsDto>.Success(stats);
        }

        public async Task<Result<MyProfileDto>> GetMyProfileAsync(string userId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Result<MyProfileDto>.Fail(ErrorType.Validation, ErrorCodes.Member.UserNotFound, "Kullanıcı ID boş olamaz.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<MyProfileDto>.Fail(ErrorType.NotFound, ErrorCodes.Member.UserNotFound, "Kullanıcı bulunamadı.");
            }

            var dto = new MyProfileDto
            {
                AdSoyad = user.AdSoyad,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                ProfilFotografiYolu = user.ProfilFotografYolu
            };

            return Result<MyProfileDto>.Success(dto);
        }

        public async Task<Result> UpdateMyProfileAsync(string userId, UpdateProfileRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Result.Fail(ErrorType.Validation, ErrorCodes.Member.UserNotFound, "Kullanıcı ID boş olamaz.");
            }

            // Validasyon
            var validationResult = await _profileValidator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return Result.FromValidation(validationResult);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result.Fail(ErrorType.NotFound, ErrorCodes.Member.UserNotFound, "Kullanıcı bulunamadı.");
            }

            user.AdSoyad = request.AdSoyad;
            
            // Normalize phone number (clean spaces/chars) before saving
            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                user.PhoneNumber = Regex.Replace(request.PhoneNumber, @"\D", "");
            }
            else
            {
                user.PhoneNumber = null;
            }

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                return Result.Fail(ErrorType.Validation, ErrorCodes.Member.UpdateFailed, $"Güncelleme başarısız: {errors}");
            }

            return Result.Success();
        }
    }
}
