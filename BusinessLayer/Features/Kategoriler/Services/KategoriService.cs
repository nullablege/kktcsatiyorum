using AutoMapper;
using BusinessLayer.Common.Constants;
using BusinessLayer.Common.Results;
using BusinessLayer.Features.Kategoriler.DTOs;
using BusinessLayer.Features.Kategoriler.Validators;
using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using FluentValidation;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessLayer.Common.Constants.ErrorCodes;

namespace BusinessLayer.Features.Kategoriler.Services
{
    public sealed class KategoriService:IKategoriService
    {
        private readonly IKategoriDal _kategoriDal;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateKategoriRequest> _validator;
        private readonly IKategoriSlugService _kategoriSlugService;
        private readonly IMapper _mapper;
        public KategoriService(IKategoriDal kategoriDal, 
                               IUnitOfWork unitOfWork,
                               IValidator<CreateKategoriRequest> validator,
                               IKategoriSlugService kategoriSlugService,
                               IMapper mapper
        )
        {
            _kategoriSlugService = kategoriSlugService;
            _kategoriDal = kategoriDal;
            _unitOfWork = unitOfWork;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<Result<CreateKategoriResponse>> CreateAsync(CreateKategoriRequest request, CancellationToken ct =  default)
        {
            var validation = await _validator.ValidateAsync(request, ct);
            if(!validation.IsValid)
                return Result<CreateKategoriResponse>.FromValidation(validation);

            if(request.UstKategoriId != null)
            {
                var parent = await _kategoriDal.GetByIdAsync(request.UstKategoriId.Value, ct);
                if (parent == null)
                    return Result<CreateKategoriResponse>.Fail(ErrorType.NotFound, ErrorCodes.Kategori.ParentNotFound, "Üst Kategori Bulunamadı");
            }

            //Slug Artık otomatik üretilecek. 
            //var sameSlug = await _kategoriDal.GetListAllAsync(x => x.SeoSlug == request.SeoSlug);
            //if (sameSlug.Count > 0)
            //    return Result<CreateKategoriResponse>.Fail(ErrorType.Conflict, ErrorCodes.Kategori.SlugExists , "Bu SeoSlug zaten kullanılıyor");

            var seoSlug = await _kategoriSlugService.GenerateUniqueAsync(request.Ad, ct);


            var kategori = new EntityLayer.Entities.Kategori
            {
                Ad = request.Ad.Trim(),
                SeoSlug = seoSlug,
                UstKategoriId = request.UstKategoriId,
                SiraNo = request.SiraNo,
                AktifMi = true
            };

            await _kategoriDal.InsertAsync(kategori, ct);

            try
            {
                await _unitOfWork.CommitAsync(ct);
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                return Result<CreateKategoriResponse>
                       .Fail(ErrorType.Conflict, ErrorCodes.Kategori.SlugExists, "Slug üretimi sırasında çakışma oldu, tekrar deneyin.");
            }
            
            return Result<CreateKategoriResponse>.Success(new CreateKategoriResponse(kategori.Id));
        }

        public async Task<Result<IReadOnlyList<KategoriListItemDto>>> GetListAsync(CancellationToken ct = default)
        {
            var list = await _kategoriDal.GetListAllAsync(ct: ct);

            var returnList = list.Select(x => new KategoriListItemDto
            {
                Id = x.Id,
                Ad = x.Ad,
                SeoSlug = x.SeoSlug,
                UstKategoriId = x.UstKategoriId,
                SiraNo = x.SiraNo,
                AktifMi = x.AktifMi
            }).ToList();

            return Result<IReadOnlyList<KategoriListItemDto>>.Success(returnList);
        }
        public async Task<Result<IReadOnlyList<KategoriDropdownItemDto>>> GetForDropdownAsync(CancellationToken ct = default)
        {
        var list = await _kategoriDal.GetListAllAsync(x => x.UstKategoriId == null,ct);
            var returnList = list.Select(x => new KategoriDropdownItemDto
            {
                Id = x.Id,
                Ad = x.Ad,
                UstKategoriId = x.UstKategoriId,
                SiraNo = x.SiraNo
            }).ToList();
            
            return Result<IReadOnlyList<KategoriDropdownItemDto>>.Success(returnList);
        }
        public async Task<Result<KategoriDetailDto>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            if ( id <= 0)
            {
                return Result<KategoriDetailDto>.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "ID Geçersiz.");
            }
            var kategori = await _kategoriDal.GetByIdAsync(id, ct);

            if( kategori == null)
            {
                return Result<KategoriDetailDto>.Fail(ErrorType.NotFound, ErrorCodes.Kategori.NotFound, "Kategori bulunamadı.");
            }

            var kategoriReturn = _mapper.Map<KategoriDetailDto>(kategori); // Automapper 


            return Result<KategoriDetailDto>.Success(kategoriReturn);
        }
        public async Task<Result<UpdateKategoriResponse>> UpdateAsync(UpdateKategoriRequest request, CancellationToken ct = default)
        {
            if(request.Id == request.UstKategoriId)
            {
                return Result<UpdateKategoriResponse>.Fail(ErrorType.Conflict, ErrorCodes.Kategori.ParentConflict, "Kategori, kendi UstKategorisi olamaz.");
            }
            var kategori = await _kategoriDal.GetByIdAsync(request.Id, ct);
            if (kategori == null)
            {
                return Result<UpdateKategoriResponse>.Fail(ErrorType.NotFound, ErrorCodes.Kategori.NotFound , "Kategori bulunamadı");
            }
            if (request.UstKategoriId != null)
            {
                var ustKategori = await _kategoriDal.GetByIdAsync(request.UstKategoriId, ct);
                if (ustKategori == null)
                    return Result<UpdateKategoriResponse>.Fail(ErrorType.NotFound, ErrorCodes.Kategori.ParentNotFound, "Ust Kategorisi Null");
            }

            kategori.Ad = request.Ad;
            kategori.UstKategoriId = request.UstKategoriId;
            kategori.SiraNo = request.SiraNo;
            kategori.AktifMi = request.AktifMi;

            try
            {
               await _unitOfWork.CommitAsync(ct);
            }
            catch
            {
                return Result<UpdateKategoriResponse>.Fail(ErrorType.Failure, ErrorCodes.Common.CommitFail, "Commit sırasında hata");
            }
            return Result<UpdateKategoriResponse>.Success(new UpdateKategoriResponse { Id = request.Id });


        }
        public async Task<Result<SoftDeleteKategoriResponse>> SoftDeleteAsync(int id, CancellationToken ct = default)
        {
            if(id <= 0)
            {
                return Result<SoftDeleteKategoriResponse>.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Girilen ID Geçersiz.");
            }
            var kategori = await _kategoriDal.GetByIdAsync(id, ct);
            if(kategori == null)
            {
                return Result<SoftDeleteKategoriResponse>.Fail(ErrorType.NotFound, ErrorCodes.Kategori.NotFound, "Kategori bulunamadı.");
            }

            kategori.AktifMi = false;
            kategori.SilindiMi = true;

            try {  
                await _unitOfWork.CommitAsync(ct);
            }
            catch
            {
                return Result<SoftDeleteKategoriResponse>.Fail(ErrorType.Failure, ErrorCodes.Common.CommitFail, "Commit sırasında hata");
            }

            return Result<SoftDeleteKategoriResponse>.Success(new SoftDeleteKategoriResponse { Id = kategori.Id });

        }
        public async Task<Result<IReadOnlyList<KategoriListItemDto>>> GetChildrensAsync(int UstKategorıId, CancellationToken ct = default)
        {
            if (UstKategorıId <= 0)
                return Result<IReadOnlyList<KategoriListItemDto>>.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Girilen ID Geçersiz");

            var UstKategori = await _kategoriDal.GetByIdAsync(UstKategorıId,ct);
            if (UstKategori == null)
            {
                return Result<IReadOnlyList<KategoriListItemDto>>.Fail(ErrorType.NotFound, ErrorCodes.Kategori.NotFound, "Ust Kategori bulunamadı");
            }
            var childrens = await _kategoriDal.GetChildrenAsync(UstKategorıId, ct);

            var returnList = childrens.Select(x => new KategoriListItemDto
            {
                Id = x.Id,
                Ad = x.Ad,
                SeoSlug = x.SeoSlug,
                UstKategoriId = x.UstKategoriId,
                SiraNo = x.SiraNo,
                AktifMi = x.AktifMi,
            }).ToList();

            return Result<IReadOnlyList<KategoriListItemDto>>.Success(returnList);

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
