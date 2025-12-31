
using BusinessLayer.Features.Member.DTOs;
using BusinessLayer.Features.Member.Services;
using BusinessLayer.Common.Results;
using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using EntityLayer.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Features.Member
{
    public class MemberService : IMemberService
    {
        private readonly IIlanDal _ilanDal;
        private readonly IFavoriDal _favoriDal;
        private readonly IBildirimDal _bildirimDal;
        private readonly UserManager<UygulamaKullanicisi> _userManager;

        public MemberService(
            IIlanDal ilanDal, 
            IFavoriDal favoriDal, 
            IBildirimDal bildirimDal, 
            UserManager<UygulamaKullanicisi> userManager)
        {
            _ilanDal = ilanDal;
            _favoriDal = favoriDal;
            _bildirimDal = bildirimDal;
            _userManager = userManager;
        }

        public async Task<Result<MemberDashboardStatsDto>> GetDashboardStatsAsync(string userId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Result<MemberDashboardStatsDto>.Fail(ErrorType.Validation, "User", "Kullanıcı ID boş olamaz.");
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
               return Result<MyProfileDto>.Fail(ErrorType.Validation, "User", "Kullanıcı ID boş olamaz.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<MyProfileDto>.Fail(ErrorType.NotFound, "User", "Kullanıcı bulunamadı.");
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
                 return Result.Fail(ErrorType.Validation, "User", "Kullanıcı ID boş olamaz.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                 return Result.Fail(ErrorType.NotFound, "User", "Kullanıcı bulunamadı.");
            }

            // Validation
            if (string.IsNullOrWhiteSpace(request.AdSoyad) || request.AdSoyad.Length < 2)
            {
                 return Result.Fail(ErrorType.Validation, "AdSoyad", "Ad Soyad en az 2 karakter olmalıdır.");
            }

            // Simple Phone Validation
            if (!string.IsNullOrEmpty(request.PhoneNumber) && (!request.PhoneNumber.All(char.IsDigit) || request.PhoneNumber.Length < 10))
            {
                 return Result.Fail(ErrorType.Validation, "PhoneNumber", "Geçersiz telefon formatı.");
            }

            user.AdSoyad = request.AdSoyad;
            user.PhoneNumber = request.PhoneNumber;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                return Result.Fail(ErrorType.Validation, "UserUpdate", $"Güncelleme başarısız: {errors}");
            }

            return Result.Success();
        }
    }
}
