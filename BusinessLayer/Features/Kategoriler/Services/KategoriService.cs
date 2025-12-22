using BusinessLayer.Common.Constants;
using BusinessLayer.Common.Results;
using BusinessLayer.Features.Kategoriler.DTOs;
using BusinessLayer.Features.Kategoriler.Validators;
using DataAccessLayer.Abstract;
using EntityLayer.Entities;
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
        private readonly CreateKategoriValidator _validator;

        public KategoriService(IKategoriDal kategoriDal, 
                               IUnitOfWork unitOfWork
        )
        {
            _kategoriDal = kategoriDal;
            _unitOfWork = unitOfWork;
            _validator = new CreateKategoriValidator();
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
            
            var sameSlug = await _kategoriDal.GetListAllAsync(x => x.SeoSlug == request.SeoSlug);
            if (sameSlug.Count > 0)
                return Result<CreateKategoriResponse>.Fail(ErrorType.Conflict, ErrorCodes.Kategori.SlugExists , "Bu SeoSlug zaten kullanılıyor");

            var kategori = new Kategori
            {
                Ad = request.Ad,
                SeoSlug = request.SeoSlug,
                UstKategoriId = request.UstKategoriId,
                SiraNo = request.SiraNo,
                AktifMi = true
            };

            await _kategoriDal.InsertAsync(kategori);
            await _unitOfWork.CommitAsync(ct);
            return Result<CreateKategoriResponse>.Success(new CreateKategoriResponse(kategori.Id));
        }
    }
}
