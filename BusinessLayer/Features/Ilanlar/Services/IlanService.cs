using AutoMapper;
using BusinessLayer.Common;
using BusinessLayer.Common.Constants;
using BusinessLayer.Common.Results;
using BusinessLayer.Features.Ilanlar.DTOs;
using DataAccessLayer.Abstract;
using EntityLayer.DTOs.Public;
using EntityLayer.Entities;
using EntityLayer.Enums;
using FluentValidation;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using BusinessLayer.Common.Abstractions;
using BusinessLayer.Common.DTOs;
using BusinessLayer.Features.DenetimKayitlari.Services;

using Microsoft.Extensions.Logging;

namespace BusinessLayer.Features.Ilanlar.Services
{
    public sealed class IlanService : IIlanService
    {
        private const string DetailCacheKeyPrefix = "listing:detail:";
        private static readonly TimeSpan DetailCacheTtl = TimeSpan.FromMinutes(5);

        private readonly IIlanDal _ilanDal;
        private readonly IBildirimDal _bildirimDal;
        private readonly IKategoriAlaniDal _kategoriAlaniDal;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateIlanRequest> _createValidator;
        private readonly ICacheService _cache;
        private readonly IMapper _mapper;
        private readonly INotificationPublisher _notificationPublisher;
        private readonly IDenetimKaydiService _denetimKaydiService;
        private readonly ILogger<IlanService> _logger;
        private readonly IValidator<UpdateIlanRequest> _updateValidator;

        public IlanService(
            IIlanDal ilanDal,
            IBildirimDal bildirimDal,
            IKategoriAlaniDal kategoriAlaniDal,
            IUnitOfWork unitOfWork,
            IValidator<CreateIlanRequest> createValidator,
            IValidator<UpdateIlanRequest> updateValidator,
            ICacheService cache,
            IMapper mapper,
            INotificationPublisher notificationPublisher,
            IDenetimKaydiService denetimKaydiService,
            ILogger<IlanService> logger,
            IContentModerationClient moderationClient)
        {
            _ilanDal = ilanDal;
            _bildirimDal = bildirimDal;
            _kategoriAlaniDal = kategoriAlaniDal;
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _cache = cache;
            _mapper = mapper;
            _notificationPublisher = notificationPublisher;
            _denetimKaydiService = denetimKaydiService;
            _logger = logger;
            _moderationClient = moderationClient;
        }

        private readonly IContentModerationClient _moderationClient;

        public async Task<Result<int>> CreateAsync(CreateIlanRequest request, string userId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result<int>.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Kullanıcı ID boş olamaz.");

            // Moderation Check
            var moderationDecision = await _moderationClient.ModerateListingAsync(request.Baslik, request.Aciklama, ct);
            if (!moderationDecision.IsAllowed)
            {
                if (moderationDecision.ReasonCode == "UNAVAILABLE")
                {
                    return Result<int>.Fail(ErrorType.Conflict, ErrorCodes.Moderation.Unavailable, moderationDecision.ReasonMessage ?? "Moderasyon servisine ulaşılamıyor.");
                }

                await _denetimKaydiService.LogAsync("ModerationBlocked", "Ilan", "0", 
                    $"İlan oluşturma moderasyona takıldı. Sebep: {moderationDecision.ReasonCode} - {moderationDecision.ReasonMessage}", 
                    null, userId, ct);

                return Result<int>.Fail(ErrorType.Validation, ErrorCodes.Moderation.Blocked, $"İlan metni moderasyona takıldı: {moderationDecision.ReasonMessage}");
            }

            var validationResult = await _createValidator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                return Result<int>.FromValidation(validationResult);

            // EAV alanlarını çek ve doğrula
            var kategoriAlanlari = await _kategoriAlaniDal.GetListByKategoriAsync(request.KategoriId, includeSecenekler: true, ct);
            
            // Duplicate attribute kontrolü
            var duplicateAttr = request.Attributes
                .GroupBy(x => x.KategoriAlaniId)
                .FirstOrDefault(g => g.Count() > 1);
            if (duplicateAttr != null)
                return Result<int>.Fail(ErrorType.Validation, ErrorCodes.Ilan.DuplicateAttribute, "Aynı alan birden fazla kez gönderilemez.");

            var eavValidation = ValidateEavAttributes(kategoriAlanlari, request.Attributes);
            if (!eavValidation.IsSuccess)
                return Result<int>.Fail(eavValidation.Error!.Type, eavValidation.Error.Code, eavValidation.Error.Message);

            // Slug üret
            var slug = GenerateSlug(request.Baslik);
            var slugExists = await _ilanDal.AnyAsync(x => x.SeoSlug == slug && !x.SilindiMi, ct);
            if (slugExists)
                slug = $"{slug}-{Guid.NewGuid().ToString()[..8]}";

            // Ilan entity
            var ilan = new Ilan
            {
                SahipKullaniciId = userId,
                KategoriId = request.KategoriId,
                Baslik = request.Baslik.Trim(),
                SeoSlug = slug,
                Aciklama = request.Aciklama.Trim(),
                Fiyat = request.Fiyat,
                ParaBirimi = request.ParaBirimi,
                Sehir = request.Sehir?.Trim(),
                Ilce = request.Ilce?.Trim(),
                Enlem = request.Enlem,
                Boylam = request.Boylam,
                Durum = IlanDurumu.OnayBekliyor,
                OlusturmaTarihi = DateTime.UtcNow
            };

            // EAV değerlerini ekle
            foreach (var attr in request.Attributes)
            {
                var alan = kategoriAlanlari.FirstOrDefault(a => a.Id == attr.KategoriAlaniId);
                if (alan == null) continue;

                var deger = ParseEavValue(alan, attr.Value);
                if (deger != null)
                    ilan.AlanDegerleri.Add(deger);
            }

            // Fotoğrafları ekle
            for (int i = 0; i < request.PhotoPaths.Count; i++)
            {
                ilan.Fotografler.Add(new IlanFotografi
                {
                    DosyaYolu = request.PhotoPaths[i],
                    KapakMi = i == 0,
                    SiraNo = i + 1,
                    OlusturmaTarihi = DateTime.UtcNow
                });
            }

            await _ilanDal.InsertAsync(ilan, ct);

            try
            {
                await _unitOfWork.CommitAsync(ct);
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                return Result<int>.Fail(ErrorType.Conflict, ErrorCodes.Ilan.DuplicateSlug, "Slug çakışması oluştu.");
            }

            return Result<int>.Success(ilan.Id);
        }

        private Result ValidateEavAttributes(List<KategoriAlani> alanlari, List<AttributeValueInput> inputs)
        {
            foreach (var alan in alanlari.Where(a => a.ZorunluMu))
            {
                var input = inputs.FirstOrDefault(i => i.KategoriAlaniId == alan.Id);
                if (input == null || string.IsNullOrWhiteSpace(input.Value))
                {
                    return Result.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, $"'{alan.Ad}' alanı zorunludur.");
                }

                // TekSecim seçenek doğrulaması
                if (alan.VeriTipi == VeriTipi.TekSecim)
                {
                    if (!int.TryParse(input.Value, out var secenekId))
                        return Result.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, $"'{alan.Ad}' için geçersiz seçenek.");

                    var validSecenek = alan.Secenekler.Any(s => s.Id == secenekId && s.AktifMi);
                    if (!validSecenek)
                        return Result.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, $"'{alan.Ad}' için geçersiz seçenek.");
                }

                // Sayı/tarih parse doğrulaması
                if (alan.VeriTipi == VeriTipi.TamSayi && !long.TryParse(input.Value, out _))
                    return Result.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, $"'{alan.Ad}' için geçerli bir tam sayı girin.");

                if (alan.VeriTipi == VeriTipi.Ondalik && !decimal.TryParse(input.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                    return Result.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, $"'{alan.Ad}' için geçerli bir sayı girin.");

                if (alan.VeriTipi == VeriTipi.Tarih && !DateTime.TryParse(input.Value, out _))
                    return Result.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, $"'{alan.Ad}' için geçerli bir tarih girin.");
            }

            return Result.Success();
        }

        private static IlanAlanDegeri? ParseEavValue(KategoriAlani alan, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var deger = new IlanAlanDegeri
            {
                KategoriAlaniId = alan.Id
            };

            bool parsed = false;
            switch (alan.VeriTipi)
            {
                case VeriTipi.Metin:
                    deger.MetinDeger = value.Trim();
                    parsed = true;
                    break;
                case VeriTipi.TamSayi:
                    if (long.TryParse(value, out var tamSayi))
                    {
                        deger.TamSayiDeger = tamSayi;
                        parsed = true;
                    }
                    break;
                case VeriTipi.Ondalik:
                    if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var ondalik))
                    {
                        deger.OndalikDeger = ondalik;
                        parsed = true;
                    }
                    break;
                case VeriTipi.DogruYanlis:
                    deger.DogruYanlisDeger = value.Equals("true", StringComparison.OrdinalIgnoreCase) || value == "1";
                    parsed = true;
                    break;
                case VeriTipi.Tarih:
                    if (DateTime.TryParse(value, out var tarih))
                    {
                        deger.TarihDeger = tarih;
                        parsed = true;
                    }
                    break;
                case VeriTipi.TekSecim:
                    if (int.TryParse(value, out var secenekId))
                    {
                        deger.SecenekId = secenekId;
                        parsed = true;
                    }
                    break;
            }

            return parsed ? deger : null;
        }

        private static string GenerateSlug(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return Guid.NewGuid().ToString()[..8];

            var slug = title.ToLowerInvariant();
            
            // Türkçe karakter dönüşümü
            slug = slug.Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u")
                       .Replace("ş", "s").Replace("ö", "o").Replace("ç", "c")
                       .Replace("İ", "i").Replace("Ğ", "g").Replace("Ü", "u")
                       .Replace("Ş", "s").Replace("Ö", "o").Replace("Ç", "c");

            // Alfanümerik olmayan karakterleri tire ile değiştir
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-");
            slug = Regex.Replace(slug, @"-+", "-");
            slug = slug.Trim('-');

            if (slug.Length > 200)
                slug = slug[..200];

            return string.IsNullOrEmpty(slug) ? Guid.NewGuid().ToString()[..8] : slug;
        }

        private static bool IsUniqueViolation(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
                return sqlEx.Number == 2601 || sqlEx.Number == 2627;
            return false;
        }

        public async Task<Result<PagedResult<ListingCardDto>>> SearchAsync(ListingSearchQuery query, CancellationToken ct = default)
        {
            // Location Validation
            if (query.UserLat.HasValue || query.UserLng.HasValue)
            {
                if (query.UserLat.HasValue && (query.UserLat.Value < -90 || query.UserLat.Value > 90))
                    return Result<PagedResult<ListingCardDto>>.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Geçersiz enlem (latitude) değeri.");

                if (query.UserLng.HasValue && (query.UserLng.Value < -180 || query.UserLng.Value > 180))
                    return Result<PagedResult<ListingCardDto>>.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Geçersiz boylam (longitude) değeri.");
            }

            if (query.MaxDistanceKm.HasValue && (query.MaxDistanceKm.Value < 1 || query.MaxDistanceKm.Value > 500))
            {
                return Result<PagedResult<ListingCardDto>>.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Mesafe filtresi 1-500 km arasında olmalıdır.");
            }

            var result = await _ilanDal.SearchPublicAsync(query, ct);
            return Result<PagedResult<ListingCardDto>>.Success(result);
        }

        public async Task<Result<ListingDetailDto>> GetPublicDetailBySlugAsync(string slug, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return Result<ListingDetailDto>.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Slug boş olamaz.");

            // Cache-aside pattern
            var cacheKey = DetailCacheKeyPrefix + slug;
            var cached = _cache.Get<ListingDetailDto>(cacheKey);
            if (cached != null)
                return Result<ListingDetailDto>.Success(cached);

            var detail = await _ilanDal.GetPublicDetailBySlugAsync(slug, ct);
            if (detail == null)
                return Result<ListingDetailDto>.Fail(ErrorType.NotFound, ErrorCodes.Ilan.NotFound, "İlan bulunamadı.");

            _cache.Set(cacheKey, detail, DetailCacheTtl);
            return Result<ListingDetailDto>.Success(detail);
        }

        public async Task<Result<PagedResult<PendingListingDto>>> GetPendingApprovalsAsync(int page, int pageSize, CancellationToken ct = default)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 50);

            var projectionResult = await _ilanDal.GetPendingApprovalsAsync(page, pageSize, ct);
            var dtoItems = _mapper.Map<List<PendingListingDto>>(projectionResult.Items);
            var result = new PagedResult<PendingListingDto>(dtoItems, projectionResult.TotalCount, projectionResult.Page, projectionResult.PageSize);
            return Result<PagedResult<PendingListingDto>>.Success(result);
        }

        public async Task<Result> ApproveAsync(int listingId, string adminUserId, CancellationToken ct = default)
        {
            if (listingId <= 0)
                return Result.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Geçersiz ilan ID.");
            if (string.IsNullOrWhiteSpace(adminUserId))
                return Result.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Admin kullanıcı ID boş olamaz.");

            var ilan = await _ilanDal.GetByIdAsync(listingId, ct);
            if (ilan == null)
                return Result.Fail(ErrorType.NotFound, ErrorCodes.Ilan.NotFound, "İlan bulunamadı.");

            if (ilan.Durum != IlanDurumu.OnayBekliyor)
                return Result.Fail(ErrorType.Conflict, ErrorCodes.Ilan.InvalidState, "İlan zaten onaylanmış veya reddedilmiş durumda.");

            ilan.Durum = IlanDurumu.Yayinda;
            ilan.YayinTarihi = DateTime.UtcNow;
            ilan.OnaylayanKullaniciId = adminUserId;
            ilan.OnayTarihi = DateTime.UtcNow;

            await _ilanDal.UpdateAsync(ilan, ct);

            var bildirim = new Bildirim
            {
                KullaniciId = ilan.SahipKullaniciId,
                Tur = BildirimTuru.IlanOnaylandi,
                VeriJson = JsonSerializer.Serialize(new { ilanId = ilan.Id, slug = ilan.SeoSlug, baslik = ilan.Baslik }),
                OkunduMu = false,
                OlusturmaTarihi = DateTime.UtcNow
            };
            await _bildirimDal.InsertAsync(bildirim, ct);

            await _unitOfWork.CommitAsync(ct);

            InvalidateListingCaches(ilan.SeoSlug);

            // SignalR Notification (Best-effort)
            try
            {
                var payload = new NotificationPushedDto
                {
                    Id = bildirim.Id,
                    Tur = BildirimTuru.IlanOnaylandi.ToString(),
                    VeriJson = bildirim.VeriJson,
                    OlusturmaTarihi = bildirim.OlusturmaTarihi,
                    Mesaj = "İlanınız onaylandı!"
                };
                await _notificationPublisher.PublishAsync(ilan.SahipKullaniciId, payload, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "SignalR notification failed for user {UserId}", ilan.SahipKullaniciId);
            }

            return Result.Success();
        }

        public async Task<Result> RejectAsync(int listingId, string adminUserId, string redNedeni, CancellationToken ct = default)
        {
            if (listingId <= 0)
                return Result.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Geçersiz ilan ID.");
            if (string.IsNullOrWhiteSpace(adminUserId))
                return Result.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Admin kullanıcı ID boş olamaz.");
            if (string.IsNullOrWhiteSpace(redNedeni))
                return Result.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Red sebebi boş olamaz.");
            if (redNedeni.Length > 200)
                return Result.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Red sebebi en fazla 200 karakter olabilir.");

            var ilan = await _ilanDal.GetByIdAsync(listingId, ct);
            if (ilan == null)
                return Result.Fail(ErrorType.NotFound, ErrorCodes.Ilan.NotFound, "İlan bulunamadı.");

            if (ilan.Durum != IlanDurumu.OnayBekliyor)
                return Result.Fail(ErrorType.Conflict, ErrorCodes.Ilan.InvalidState, "İlan zaten onaylanmış veya reddedilmiş durumda.");

            ilan.Durum = IlanDurumu.Reddedildi;
            ilan.RedNedeni = redNedeni;

            await _ilanDal.UpdateAsync(ilan, ct);

            var bildirim = new Bildirim
            {
                KullaniciId = ilan.SahipKullaniciId,
                Tur = BildirimTuru.IlanReddedildi,
                VeriJson = JsonSerializer.Serialize(new { ilanId = ilan.Id, slug = ilan.SeoSlug, baslik = ilan.Baslik, redNedeni }),
                OkunduMu = false,
                OlusturmaTarihi = DateTime.UtcNow
            };
            await _bildirimDal.InsertAsync(bildirim, ct);

            await _unitOfWork.CommitAsync(ct);

            InvalidateListingCaches(ilan.SeoSlug);

            // SignalR Notification (Best-effort)
            try
            {
                var payload = new NotificationPushedDto
                {
                    Id = bildirim.Id,
                    Tur = BildirimTuru.IlanReddedildi.ToString(),
                    VeriJson = bildirim.VeriJson,
                    OlusturmaTarihi = bildirim.OlusturmaTarihi,
                    Mesaj = $"İlanınız reddedildi: {redNedeni}"
                };
                await _notificationPublisher.PublishAsync(ilan.SahipKullaniciId, payload, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "SignalR notification failed for user {UserId}", ilan.SahipKullaniciId);
            }

            return Result.Success();
        }

        public async Task<Result<PagedResult<MyListingDto>>> GetMyListingsAsync(string userId, int page, int pageSize, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result<PagedResult<MyListingDto>>.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Kullanıcı ID boş olamaz.");

            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 50);

            var projectionResult = await _ilanDal.GetUserListingsAsync(userId, page, pageSize, ct);
            var dtoItems = _mapper.Map<List<MyListingDto>>(projectionResult.Items);
            var result = new PagedResult<MyListingDto>(dtoItems, projectionResult.TotalCount, projectionResult.Page, projectionResult.PageSize);
            return Result<PagedResult<MyListingDto>>.Success(result);
        }

        public async Task<Result> DeleteMyListingAsync(int listingId, string userId, CancellationToken ct = default)
        {
            if (listingId <= 0)
                return Result.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Geçersiz ilan ID.");
            if (string.IsNullOrWhiteSpace(userId))
                return Result.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Kullanıcı ID boş olamaz.");

            var ilan = await _ilanDal.GetByIdAsync(listingId, ct);
            if (ilan == null)
                return Result.Fail(ErrorType.NotFound, ErrorCodes.Ilan.NotFound, "İlan bulunamadı.");

            if (ilan.SahipKullaniciId != userId)
                return Result.Fail(ErrorType.Forbidden, ErrorCodes.Common.Forbidden, "Bu ilana erişim yetkiniz yok.");

            if (ilan.SilindiMi)
                return Result.Fail(ErrorType.NotFound, ErrorCodes.Ilan.NotFound, "İlan zaten silinmiş.");

            ilan.SilindiMi = true;
            await _ilanDal.UpdateAsync(ilan, ct);
            await _unitOfWork.CommitAsync(ct);

            InvalidateListingCaches(ilan.SeoSlug);

            return Result.Success();
        }

        public async Task<Result<EditIlanDto>> GetMyListingForEditAsync(int ilanId, string userId, CancellationToken ct = default)
        {
            if (ilanId <= 0)
                return Result<EditIlanDto>.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Geçersiz ilan ID.");
            if (string.IsNullOrWhiteSpace(userId))
                return Result<EditIlanDto>.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Kullanıcı ID boş olamaz.");

            var ilan = await _ilanDal.GetForEditAsync(ilanId, userId, ct);
            if (ilan == null)
            {
                // Either not found or not owned by user
                return Result<EditIlanDto>.Fail(ErrorType.NotFound, ErrorCodes.Ilan.NotFound, "İlan bulunamadı veya erişim yetkiniz yok.");
            }

            // Map to DTO
            var attributes = ilan.AlanDegerleri.Select(ad => new EditAttributeDto(
                ad.KategoriAlaniId,
                ad.KategoriAlani?.Ad ?? "",
                GetRawValue(ad),
                ad.KategoriAlani?.VeriTipi ?? VeriTipi.Metin,
                ad.KategoriAlani?.Secenekler?.Select(s => new SecenekDto { Id = s.Id, Deger = s.Deger }).ToList()
            )).ToList();

            var dto = new EditIlanDto(
                ilan.Id,
                ilan.KategoriId,
                ilan.Baslik,
                ilan.Aciklama,
                ilan.Fiyat,
                ilan.ParaBirimi,
                ilan.Sehir,
                ilan.Ilce,
                ilan.Enlem,
                ilan.Boylam,
                attributes,
                ilan.Fotografler.OrderBy(f => f.SiraNo).Select(f => new PhotoDto(f.DosyaYolu, f.KapakMi, f.SiraNo)).ToList()
            );

            return Result<EditIlanDto>.Success(dto);
        }

        public async Task<Result<MemberListingDetailDto>> GetMemberListingDetailAsync(int ilanId, string userId, CancellationToken ct = default)
        {
            if (ilanId <= 0)
                return Result<MemberListingDetailDto>.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Geçersiz ilan ID.");
            if (string.IsNullOrWhiteSpace(userId))
                return Result<MemberListingDetailDto>.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Kullanıcı ID boş olamaz.");

            var ilan = await _ilanDal.GetForEditAsync(ilanId, userId, ct);
            if (ilan == null)
            {
                return Result<MemberListingDetailDto>.Fail(ErrorType.NotFound, ErrorCodes.Ilan.NotFound, "İlan bulunamadı veya erişim yetkiniz yok.");
            }

            var attributes = ilan.AlanDegerleri.Select(ad => new EditAttributeDto(
                 ad.KategoriAlaniId,
                 ad.KategoriAlani?.Ad ?? "",
                 GetRawValue(ad),
                 ad.KategoriAlani?.VeriTipi ?? VeriTipi.Metin,
                 ad.KategoriAlani?.Secenekler?.Select(s => new SecenekDto { Id = s.Id, Deger = s.Deger }).ToList()
             )).ToList();

            var dto = new MemberListingDetailDto(
                ilan.Id,
                ilan.KategoriId,
                ilan.Baslik,
                ilan.Aciklama,
                ilan.Fiyat,
                ilan.ParaBirimi,
                ilan.Sehir,
                ilan.Ilce,
                ilan.Enlem,
                ilan.Boylam,
                attributes,
                ilan.Fotografler.OrderBy(f => f.SiraNo).Select(f => new PhotoDto(f.DosyaYolu, f.KapakMi, f.SiraNo)).ToList(),
                ilan.Durum,
                ilan.RedNedeni,
                ilan.SeoSlug,
                ilan.OlusturmaTarihi,
                ilan.YayinTarihi
            );

            return Result<MemberListingDetailDto>.Success(dto);
        }

        private static string? GetRawValue(IlanAlanDegeri ad)
        {
             if (ad.SecenekId.HasValue) return ad.SecenekId.Value.ToString();
             if (ad.TamSayiDeger.HasValue) return ad.TamSayiDeger.Value.ToString();
             if (ad.OndalikDeger.HasValue) return ad.OndalikDeger.Value.ToString(CultureInfo.InvariantCulture); // Culture invariant for form binding
             if (ad.DogruYanlisDeger.HasValue) return ad.DogruYanlisDeger.Value ? "true" : "false";
             if (ad.TarihDeger.HasValue) return ad.TarihDeger.Value.ToString("yyyy-MM-dd"); // ISO for input type=date
             return ad.MetinDeger;
        }

        public async Task<Result> UpdateMyListingAsync(int ilanId, UpdateIlanRequest request, string userId, CancellationToken ct = default)
        {
            if (ilanId <= 0)
                return Result.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Geçersiz ilan ID.");
            if (string.IsNullOrWhiteSpace(userId))
                return Result.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Kullanıcı ID boş olamaz.");

            var validationResult = await _updateValidator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                return Result.FromValidation(validationResult);

            var inputAttributes = request.Attributes ?? new List<AttributeValueInput>();

             // Duplicate attribute check
            var duplicateAttr = inputAttributes
                .GroupBy(x => x.KategoriAlaniId)
                .FirstOrDefault(g => g.Count() > 1);
            if (duplicateAttr != null)
                return Result.Fail(ErrorType.Validation, ErrorCodes.Ilan.DuplicateAttribute, "Aynı alan birden fazla kez gönderilemez.");

            var ilan = await _ilanDal.GetForEditAsync(ilanId, userId, ct);
            if (ilan == null)
                return Result.Fail(ErrorType.NotFound, ErrorCodes.Ilan.NotFound, "İlan bulunamadı veya erişim yetkiniz yok.");

            var oldSlug = ilan.SeoSlug;

            // Content Moderation (Only if title/description changed)
            if (ilan.Baslik != request.Baslik || ilan.Aciklama != request.Aciklama)
            {
                 var moderationDecision = await _moderationClient.ModerateListingAsync(request.Baslik, request.Aciklama, ct);
                 if (!moderationDecision.IsAllowed)
                 {
                     if (moderationDecision.ReasonCode == "UNAVAILABLE")
                     {
                         return Result.Fail(ErrorType.Conflict, ErrorCodes.Moderation.Unavailable, moderationDecision.ReasonMessage ?? "Moderasyon servisine ulaşılamıyor.");
                     }

                     await _denetimKaydiService.LogAsync("ModerationBlocked", "Ilan", ilan.Id.ToString(), 
                         $"İlan güncelleme moderasyona takıldı. Sebep: {moderationDecision.ReasonCode} - {moderationDecision.ReasonMessage}", 
                         null, userId, ct);

                     return Result.Fail(ErrorType.Validation, ErrorCodes.Moderation.Blocked, $"İlan metni moderasyona takıldı: {moderationDecision.ReasonMessage}");
                 }
            }

            // Check if critical fields changed

            // Check if critical fields changed
            bool criticalFieldsChanged = 
                ilan.Baslik != request.Baslik ||
                ilan.Aciklama != request.Aciklama ||
                ilan.Fiyat != request.Fiyat ||
                ilan.KategoriId != request.KategoriId ||
                ilan.ParaBirimi != request.ParaBirimi;
            
            // Check if attributes changed
            bool attributesChanged = false;
            // First check count mismatch (assuming Category didn't change, forcing clear/rewrite logic handled by ilan.KategoriId check implicitly, but explicit here is safer)
            // If category matches, we compare values. If category changed, attributesChanged is implicitly true/irrelevant as criticalFieldsChanged is true.
            // But let's calculate it correctly for completeness.
            if (ilan.KategoriId != request.KategoriId)
            {
                attributesChanged = true; // Category change implies attributes change
            }
            else
            {
                // Same category check for value diffs
                if (ilan.AlanDegerleri.Count != inputAttributes.Count)
                {
                    attributesChanged = true;
                }
                else
                {
                    foreach (var inputAttr in inputAttributes)
                    {
                        var existingAttr = ilan.AlanDegerleri.FirstOrDefault(a => a.KategoriAlaniId == inputAttr.KategoriAlaniId);
                        // Compare normalized strings (treat null and empty as same to avoid false positives)
                        var existingVal = existingAttr != null ? GetRawValue(existingAttr) : null;
                        var newVal = inputAttr.Value;

                        if ((existingVal ?? "") != (newVal ?? ""))
                        {
                            attributesChanged = true;
                            break;
                        }
                    }
                }
            }

            // Update Entity Fields
            ilan.KategoriId = request.KategoriId;
            ilan.Baslik = request.Baslik.Trim();
            ilan.Aciklama = request.Aciklama.Trim();
            ilan.Fiyat = request.Fiyat;
            ilan.ParaBirimi = request.ParaBirimi;
            ilan.Sehir = request.Sehir?.Trim();
            ilan.Ilce = request.Ilce?.Trim();
            ilan.Enlem = request.Enlem;
            ilan.Boylam = request.Boylam;
            ilan.GuncellemeTarihi = DateTime.UtcNow;

            // Update Slug if Title changed
            var newSlug = GenerateSlug(ilan.Baslik);
            if (newSlug != ilan.SeoSlug)
            {
                var slugExists = await _ilanDal.IsSlugTakenAsync(newSlug, ilan.Id, ct);
                if (slugExists)
                    newSlug = $"{newSlug}-{Guid.NewGuid().ToString()[..8]}";
                ilan.SeoSlug = newSlug;
            }

            // Update Attributes
            ilan.AlanDegerleri.Clear();
            
            var kategoriAlanlari = await _kategoriAlaniDal.GetListByKategoriAsync(request.KategoriId, includeSecenekler: true, ct);
            
             var eavValidation = ValidateEavAttributes(kategoriAlanlari, inputAttributes);
            if (!eavValidation.IsSuccess)
                return Result.Fail(eavValidation.Error!.Type, eavValidation.Error.Code, eavValidation.Error.Message);

            foreach (var attr in inputAttributes)
            {
                 var alan = kategoriAlanlari.FirstOrDefault(a => a.Id == attr.KategoriAlaniId);
                 if (alan == null) continue;
                 var deger = ParseEavValue(alan, attr.Value);
                 if (deger != null)
                     ilan.AlanDegerleri.Add(deger);
            }

            // Re-approval Logic
            if (ilan.Durum == IlanDurumu.Yayinda)
            {
                if (criticalFieldsChanged || attributesChanged)
                {
                    ilan.Durum = IlanDurumu.OnayBekliyor;
                    ilan.OnaylayanKullaniciId = null;
                    ilan.OnayTarihi = null;
                    ilan.YayinTarihi = null; 

                    await _denetimKaydiService.LogAsync("IlanGuncelleme", "Ilan", ilan.Id.ToString(), 
                        $"İlan düzenlendi ve tekrar onaya düştü. Eski durum: Yayinda.", 
                        null, userId, ct);
                }
            }
            else if (ilan.Durum == IlanDurumu.Reddedildi)
            {
                ilan.Durum = IlanDurumu.OnayBekliyor;
                ilan.RedNedeni = null;
                 await _denetimKaydiService.LogAsync("IlanGuncelleme", "Ilan", ilan.Id.ToString(), 
                     "Reddedilen ilan düzenlendi ve tekrar onaya gönderildi.", 
                     null, userId, ct);
            }

            try
            {
                await _ilanDal.UpdateAsync(ilan, ct);
                await _unitOfWork.CommitAsync(ct);
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                return Result.Fail(ErrorType.Conflict, ErrorCodes.Ilan.DuplicateSlug, "Slug çakışması oluştu.");
            }

            // Cache Invalidation
            InvalidateListingCaches(oldSlug);
            if (oldSlug != ilan.SeoSlug)
            {
                InvalidateListingCaches(ilan.SeoSlug);
            }

            return Result.Success();
        }

        private void InvalidateListingCaches(string slug)
        {
            _cache.Remove(DetailCacheKeyPrefix + slug);
            _cache.RemoveByPrefix("listing:");
        }
    }
}
