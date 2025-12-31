using AutoMapper;
using BusinessLayer.Common.Constants;
using BusinessLayer.Common.Results;
using BusinessLayer.Features.Bildirimler.DTOs;
using DataAccessLayer.Abstract;
using EntityLayer.DTOs.Public;

namespace BusinessLayer.Features.Bildirimler.Services
{
    public class BildirimService : IBildirimService
    {
        private readonly IBildirimDal _bildirimDal;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BildirimService(
            IBildirimDal bildirimDal,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _bildirimDal = bildirimDal;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResult<MyNotificationDto>>> GetMyNotificationsAsync(string userId, int page, int pageSize, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result<PagedResult<MyNotificationDto>>.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Kullanıcı ID boş olamaz.");

            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 50);

            var projectionResult = await _bildirimDal.GetUserNotificationsAsync(userId, page, pageSize, ct);
            var dtoItems = _mapper.Map<List<MyNotificationDto>>(projectionResult.Items);
            
            var result = new PagedResult<MyNotificationDto>(dtoItems, projectionResult.TotalCount, projectionResult.Page, projectionResult.PageSize);
            return Result<PagedResult<MyNotificationDto>>.Success(result);
        }

        public async Task<Result> MarkAsReadAsync(int notificationId, string userId, CancellationToken ct = default)
        {
            if (notificationId <= 0)
                return Result.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Geçersiz bildirim ID.");
            if (string.IsNullOrWhiteSpace(userId))
                return Result.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Kullanıcı ID boş olamaz.");

            var bildirim = await _bildirimDal.GetByIdAsync(notificationId, ct);
            if (bildirim == null)
                return Result.Fail(ErrorType.NotFound, ErrorCodes.Bildirim.NotFound, "Bildirim bulunamadı.");

            if (bildirim.KullaniciId != userId)
                return Result.Fail(ErrorType.Forbidden, ErrorCodes.Common.Forbidden, "Bu bildirime erişim yetkiniz yok.");

            if (bildirim.OkunduMu)
                return Result.Success(); 

            bildirim.OkunduMu = true;
            await _bildirimDal.UpdateAsync(bildirim, ct);
            await _unitOfWork.CommitAsync(ct);

            return Result.Success();
        }

        public async Task<Result<int>> GetUnreadCountAsync(string userId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result<int>.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Kullanıcı ID boş olamaz.");

            var count = await _bildirimDal.GetUnreadCountAsync(userId, ct);
            return Result<int>.Success(count);
        }
    }
}
