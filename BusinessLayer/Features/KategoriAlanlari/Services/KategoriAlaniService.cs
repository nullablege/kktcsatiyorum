using AutoMapper;
using BusinessLayer.Common.Constants;
using BusinessLayer.Common.Results;
using BusinessLayer.Features.KategoriAlanlari.DTOs;
using DataAccessLayer;
using EntityLayer.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Features.KategoriAlanlari.Services
{
    public sealed class KategoriAlaniService : IKategoriAlaniService
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateKategoriAlaniRequest> _createValidator;
        private readonly IValidator<UpdateKategoriAlaniRequest> _updateValidator;

        public KategoriAlaniService(
            Context context,
            IMapper mapper,
            IValidator<CreateKategoriAlaniRequest> createValidator,
            IValidator<UpdateKategoriAlaniRequest> updateValidator)
        {
            _context = context;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<Result<int>> CreateAsync(CreateKategoriAlaniRequest request, CancellationToken ct = default)
        {
            var validationResult = await _createValidator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                return Result<int>.FromValidation(validationResult);

            var kategoriExists = await _context.Kategoriler
                .AnyAsync(k => k.Id == request.KategoriId && !k.SilindiMi, ct);
            if (!kategoriExists)
                return Result<int>.Fail(ErrorType.NotFound, ErrorCodes.Kategori.NotFound, "Kategori bulunamadı.");

            var keyExists = await _context.KategoriAlanlari
                .AnyAsync(a => a.KategoriId == request.KategoriId && a.Anahtar == request.Anahtar, ct);
            if (keyExists)
                return Result<int>.Fail(ErrorType.Conflict, ErrorCodes.KategoriAlani.DuplicateKey, "Bu anahtar bu kategoride zaten kullanılıyor.");

            var entity = new KategoriAlani
            {
                KategoriId = request.KategoriId,
                Ad = request.Ad,
                Anahtar = request.Anahtar,
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
                        Deger = request.Secenekler[i],
                        SiraNo = i + 1,
                        AktifMi = true
                    });
                }
            }

            _context.KategoriAlanlari.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<int>.Success(entity.Id);
        }

        public async Task<Result> UpdateAsync(UpdateKategoriAlaniRequest request, CancellationToken ct = default)
        {
            var validationResult = await _updateValidator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                return Result.FromValidation(validationResult);

            var entity = await _context.KategoriAlanlari
                .FirstOrDefaultAsync(a => a.Id == request.Id, ct);
            if (entity == null)
                return Result.Fail(ErrorType.NotFound, ErrorCodes.KategoriAlani.NotFound, "Alan bulunamadı.");

            var keyExists = await _context.KategoriAlanlari
                .AnyAsync(a => a.KategoriId == entity.KategoriId && a.Anahtar == request.Anahtar && a.Id != request.Id, ct);
            if (keyExists)
                return Result.Fail(ErrorType.Conflict, ErrorCodes.KategoriAlani.DuplicateKey, "Bu anahtar bu kategoride zaten kullanılıyor.");

            entity.Ad = request.Ad;
            entity.Anahtar = request.Anahtar;
            entity.VeriTipi = request.VeriTipi;
            entity.ZorunluMu = request.ZorunluMu;
            entity.FiltrelenebilirMi = request.FiltrelenebilirMi;
            entity.SiraNo = request.SiraNo;
            entity.AktifMi = request.AktifMi;

            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }

        public async Task<Result> SoftDeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _context.KategoriAlanlari
                .FirstOrDefaultAsync(a => a.Id == id, ct);
            if (entity == null)
                return Result.Fail(ErrorType.NotFound, ErrorCodes.KategoriAlani.NotFound, "Alan bulunamadı.");

            entity.AktifMi = false;
            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }

        public async Task<Result<IReadOnlyList<KategoriAlaniListItemDto>>> GetListByKategoriAsync(int kategoriId, CancellationToken ct = default)
        {
            var list = await _context.KategoriAlanlari
                .AsNoTracking()
                .Include(a => a.Secenekler)
                .Where(a => a.KategoriId == kategoriId)
                .OrderBy(a => a.SiraNo)
                .ToListAsync(ct);

            var dtos = _mapper.Map<List<KategoriAlaniListItemDto>>(list);
            return Result<IReadOnlyList<KategoriAlaniListItemDto>>.Success(dtos);
        }

        public async Task<Result<KategoriAlaniDetailDto>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _context.KategoriAlanlari
                .AsNoTracking()
                .Include(a => a.Kategori)
                .Include(a => a.Secenekler.OrderBy(s => s.SiraNo))
                .FirstOrDefaultAsync(a => a.Id == id, ct);

            if (entity == null)
                return Result<KategoriAlaniDetailDto>.Fail(ErrorType.NotFound, ErrorCodes.KategoriAlani.NotFound, "Alan bulunamadı.");

            var dto = _mapper.Map<KategoriAlaniDetailDto>(entity);
            return Result<KategoriAlaniDetailDto>.Success(dto);
        }
    }
}
