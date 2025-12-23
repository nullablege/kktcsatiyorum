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

namespace BusinessLayer.Features.Kategoriler.Services
{
    public sealed class KategoriService:IKategoriService
    {
        private readonly IKategoriDal _kategoriDal;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateKategoriRequest> _validator;
        private readonly IKategoriSlugService _kategoriSlugService;
        public KategoriService(IKategoriDal kategoriDal, 
                               IUnitOfWork unitOfWork,
                               IValidator<CreateKategoriRequest> validator,
                               IKategoriSlugService kategoriSlugService
        )
        {
            _kategoriSlugService = kategoriSlugService;
            _kategoriDal = kategoriDal;
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<Result<CreateKategoriResponse>> CreateAsync(CreateKategoriRequest request, CancellationToken ct =  default)
        {
            var validation = await _validator.ValidateAsync(request, ct);
            if(!validation.IsValid)
                return Result<CreateKategoriResponse>.FromValidation(validation);

            if(request.UstKategoriId != null)
            {
                var parent = await _kategoriDal.GetByIdAsync(request.UstKategoriId.Value);
                if (parent == null)
                    return Result<CreateKategoriResponse>.Fail(ErrorType.NotFound, ErrorCodes.Kategori.ParentNotFound, "Üst Kategori Bulunamadı");
            }

            //Slug Artık otomatik üretilecek. 
            //var sameSlug = await _kategoriDal.GetListAllAsync(x => x.SeoSlug == request.SeoSlug);
            //if (sameSlug.Count > 0)
            //    return Result<CreateKategoriResponse>.Fail(ErrorType.Conflict, ErrorCodes.Kategori.SlugExists , "Bu SeoSlug zaten kullanılıyor");

            var seoSlug = await _kategoriSlugService.GenerateUniqueAsync(request.Ad, ct);


            var kategori = new Kategori
            {
                Ad = request.Ad.Trim(),
                SeoSlug = seoSlug,
                UstKategoriId = request.UstKategoriId,
                SiraNo = request.SiraNo,
                AktifMi = true
            };

            await _kategoriDal.InsertAsync(kategori);

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
