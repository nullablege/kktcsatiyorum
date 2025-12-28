using AutoMapper;
using BusinessLayer.Common.Constants;
using BusinessLayer.Common.Results;
using BusinessLayer.Features.KategoriAlanlari.DTOs;
using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using FluentValidation;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Features.KategoriAlanlari.Services
{
    public sealed class KategoriAlaniService : IKategoriAlaniService
    {
        private readonly IKategoriAlaniDal _kategoriAlaniDal;
        private readonly IKategoriDal _kategoriDal;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateKategoriAlaniRequest> _createValidator;
        private readonly IValidator<UpdateKategoriAlaniRequest> _updateValidator;

        public KategoriAlaniService(
            IKategoriAlaniDal kategoriAlaniDal,
            IKategoriDal kategoriDal,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<CreateKategoriAlaniRequest> createValidator,
            IValidator<UpdateKategoriAlaniRequest> updateValidator)
        {
            _kategoriAlaniDal = kategoriAlaniDal;
            _kategoriDal = kategoriDal;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<Result<int>> CreateAsync(CreateKategoriAlaniRequest request, CancellationToken ct = default)
        {
            var validationResult = await _createValidator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                return Result<int>.FromValidation(validationResult);

            var kategori = await _kategoriDal.GetByIdAsync(request.KategoriId, ct);
            if (kategori == null || kategori.SilindiMi)
                return Result<int>.Fail(ErrorType.NotFound, ErrorCodes.Kategori.NotFound, "Kategori bulunamadı.");

            var entity = new KategoriAlani
            {
                KategoriId = request.KategoriId,
                Ad = request.Ad.Trim(),
                Anahtar = request.Anahtar.Trim().ToLowerInvariant(),
                VeriTipi = request.VeriTipi,
                ZorunluMu = request.ZorunluMu,
                FiltrelenebilirMi = request.FiltrelenebilirMi,
                SiraNo = request.SiraNo,
                AktifMi = true
            };

            if (request.Secenekler != null && request.Secenekler.Count > 0)
            {
                for (int i = 0; i < request.Secenekler.Count; i++)
                {
                    entity.Secenekler.Add(new KategoriAlaniSecenegi
                    {
                        Deger = request.Secenekler[i].Trim(),
                        SiraNo = i + 1,
                        AktifMi = true
                    });
                }
            }

            await _kategoriAlaniDal.InsertAsync(entity, ct);

            try
            {
                await _unitOfWork.CommitAsync(ct);
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                return Result<int>.Fail(ErrorType.Conflict, ErrorCodes.KategoriAlani.DuplicateKey, "Bu anahtar bu kategoride zaten kullanılıyor.");
            }

            return Result<int>.Success(entity.Id);
        }

        public async Task<Result> UpdateAsync(UpdateKategoriAlaniRequest request, CancellationToken ct = default)
        {
            var validationResult = await _updateValidator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                return Result.FromValidation(validationResult);

            var entity = await _kategoriAlaniDal.GetByIdAsync(request.Id, ct);
            if (entity == null || !entity.AktifMi)
                return Result.Fail(ErrorType.NotFound, ErrorCodes.KategoriAlani.NotFound, "Alan bulunamadı.");

            entity.Ad = request.Ad.Trim();
            entity.Anahtar = request.Anahtar.Trim().ToLowerInvariant();
            entity.VeriTipi = request.VeriTipi;
            entity.ZorunluMu = request.ZorunluMu;
            entity.FiltrelenebilirMi = request.FiltrelenebilirMi;
            entity.SiraNo = request.SiraNo;
            entity.AktifMi = request.AktifMi;

            try
            {
                await _unitOfWork.CommitAsync(ct);
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                return Result.Fail(ErrorType.Conflict, ErrorCodes.KategoriAlani.DuplicateKey, "Bu anahtar bu kategoride zaten kullanılıyor.");
            }

            return Result.Success();
        }

        public async Task<Result> DeactivateAsync(int id, CancellationToken ct = default)
        {
            if (id <= 0)
                return Result.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Geçersiz ID.");

            var entity = await _kategoriAlaniDal.GetByIdAsync(id, ct);
            if (entity == null)
                return Result.Fail(ErrorType.NotFound, ErrorCodes.KategoriAlani.NotFound, "Alan bulunamadı.");

            if (!entity.AktifMi)
                return Result.Success();

            entity.AktifMi = false;

            try
            {
                await _unitOfWork.CommitAsync(ct);
            }
            catch (Exception)
            {
                return Result.Fail(ErrorType.Failure, ErrorCodes.Common.CommitFail, "Commit sırasında hata.");
            }

            return Result.Success();
        }

        public async Task<Result<IReadOnlyList<KategoriAlaniListItemDto>>> GetListByKategoriAsync(int kategoriId, CancellationToken ct = default)
        {
            if (kategoriId <= 0)
                return Result<IReadOnlyList<KategoriAlaniListItemDto>>.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Geçersiz kategori ID.");

            var list = await _kategoriAlaniDal.GetListByKategoriAsync(kategoriId, includeSecenekler: true, ct);
            var dtos = _mapper.Map<List<KategoriAlaniListItemDto>>(list);
            return Result<IReadOnlyList<KategoriAlaniListItemDto>>.Success(dtos);
        }

        public async Task<Result<KategoriAlaniDetailDto>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            if (id <= 0)
                return Result<KategoriAlaniDetailDto>.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Geçersiz ID.");

            var entity = await _kategoriAlaniDal.GetByIdWithSeceneklerAsync(id, ct);
            if (entity == null)
                return Result<KategoriAlaniDetailDto>.Fail(ErrorType.NotFound, ErrorCodes.KategoriAlani.NotFound, "Alan bulunamadı.");

            var dto = _mapper.Map<KategoriAlaniDetailDto>(entity);
            return Result<KategoriAlaniDetailDto>.Success(dto);
        }

        private static bool IsUniqueViolation(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                return sqlEx.Number == 2601 || sqlEx.Number == 2627;
            }
            return false;
        }
    }
}
