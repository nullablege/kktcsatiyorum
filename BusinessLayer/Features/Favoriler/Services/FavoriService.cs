using AutoMapper;
using BusinessLayer.Common.Constants;
using BusinessLayer.Common.Results;
using BusinessLayer.Features.Favoriler.DTOs;
using DataAccessLayer.Abstract;
using EntityLayer.DTOs.Public;
using EntityLayer.Entities;
using EntityLayer.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLayer.Features.Favoriler.Services
{
    public class FavoriService : IFavoriService
    {
        private readonly IFavoriDal _favoriDal;
        private readonly IIlanDal _ilanDal;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FavoriService(
            IFavoriDal favoriDal,
            IIlanDal ilanDal,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _favoriDal = favoriDal;
            _ilanDal = ilanDal;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PagedResult<FavoriteListingDto>>> GetMyFavoritesAsync(string userId, int page, int pageSize, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result<PagedResult<FavoriteListingDto>>.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Kullanıcı ID boş olamaz.");

            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 50);

            var pagedProjection = await _favoriDal.GetUserFavoritesAsync(userId, page, pageSize, ct);
            var dtos = _mapper.Map<List<FavoriteListingDto>>(pagedProjection.Items);

            var result = new PagedResult<FavoriteListingDto>(dtos, pagedProjection.TotalCount, pagedProjection.Page, pagedProjection.PageSize);
            return Result<PagedResult<FavoriteListingDto>>.Success(result);
        }

        public async Task<Result<bool>> IsFavoriteAsync(int ilanId, string userId, CancellationToken ct = default)
        {
             if (ilanId <= 0 || string.IsNullOrWhiteSpace(userId))
                return Result<bool>.Success(false);

             var exists = await _favoriDal.ExistsAsync(userId, ilanId, ct);
             return Result<bool>.Success(exists);
        }

        public async Task<Result<FavoriteToggleResultDto>> ToggleAsync(int ilanId, string userId, CancellationToken ct = default)
        {
            if (ilanId <= 0)
                return Result<FavoriteToggleResultDto>.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Geçersiz ilan ID.");
            
            if (string.IsNullOrWhiteSpace(userId))
                return Result<FavoriteToggleResultDto>.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Kullanıcı ID gereklidir.");

            // Ilan kontrol
            var ilan = await _ilanDal.GetByIdAsync(ilanId, ct);
            if (ilan == null)
                return Result<FavoriteToggleResultDto>.Fail(ErrorType.NotFound, ErrorCodes.Ilan.NotFound, "İlan bulunamadı.");

            if (ilan.Durum != IlanDurumu.Yayinda)
                 return Result<FavoriteToggleResultDto>.Fail(ErrorType.Conflict, ErrorCodes.Ilan.InvalidState, "Sadece yayındaki ilanlar favoriye eklenebilir.");

            // Mevcut favori kontrol
            var existingFavori = await _favoriDal.GetByUserAndListingAsync(userId, ilanId, ct);
            bool isFavoriteNow;

            if (existingFavori != null)
            {
                // Remove
                await _favoriDal.DeleteAsync(existingFavori, ct);
                isFavoriteNow = false;
            }
            else
            {
                // Add
                await _favoriDal.InsertAsync(new Favori
                {
                    IlanId = ilanId,
                    KullaniciId = userId,
                    OlusturmaTarihi = DateTime.UtcNow
                }, ct);
                isFavoriteNow = true;
            }

            await _unitOfWork.CommitAsync(ct);

            return Result<FavoriteToggleResultDto>.Success(new FavoriteToggleResultDto { IsFavorite = isFavoriteNow });
        }
    }
}
