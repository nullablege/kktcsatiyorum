using BusinessLayer.Common.Constants;
using BusinessLayer.Common.Results;
using BusinessLayer.Features.DenetimKayitlari.DTOs;
using BusinessLayer.Features.DenetimKayitlari.Services;
using DataAccessLayer.Abstract;

using EntityLayer.DTOs.Public;
using EntityLayer.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLayer.Features.DenetimKayitlari.Managers
{
    public class DenetimKaydiManager : IDenetimKaydiService
    {
        private readonly IDenetimKaydiDal _dal;
        private readonly IUnitOfWork _unitOfWork;

        public DenetimKaydiManager(IDenetimKaydiDal dal, IUnitOfWork unitOfWork)
        {
            _dal = dal;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResult<DenetimKaydiListItemDto>>> GetPagedAsync(DenetimKaydiQuery query, CancellationToken ct)
        {
            if (query.Page < 1) 
                 return Result<PagedResult<DenetimKaydiListItemDto>>.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Sayfa numarası 1'den küçük olamaz.");
            if (query.PageSize < 1 || query.PageSize > 100) 
                 return Result<PagedResult<DenetimKaydiListItemDto>>.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Sayfa boyutu 1-100 arasında olmalıdır.");

            var request = new DataAccessLayer.Requests.DenetimKaydiDalRequest
            {
                Page = query.Page,
                PageSize = query.PageSize,
                BaslangicTarihi = query.BaslangicTarihi,
                BitisTarihi = query.BitisTarihi,
                Eylem = query.Eylem,
                VarlikAdi = query.VarlikAdi,
                KullaniciId = query.KullaniciId
            };

            var pagedProjection = await _dal.GetPagedAsync(request, ct);

            // Mapping
            var dtos = pagedProjection.Items.Select(x => new DenetimKaydiListItemDto
            {
                Id = x.Id,
                Eylem = x.Eylem,
                VarlikAdi = x.VarlikAdi,
                VarlikId = x.VarlikId,
                OlusturmaTarihi = x.OlusturmaTarihi,
                KullaniciId = x.KullaniciId,
                KullaniciEmail = x.KullaniciEmail,
                IpAdresi = x.IpAdresi
            }).ToList();

            var result = new PagedResult<DenetimKaydiListItemDto>(dtos, pagedProjection.TotalCount, pagedProjection.Page, pagedProjection.PageSize);
            return Result<PagedResult<DenetimKaydiListItemDto>>.Success(result);
        }

        public async Task<Result> LogAsync(string eylem, string varlikAdi, string varlikId, string? detayJson, string? ipAdresi, string? kullaniciId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(eylem)) 
                return Result.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Eylem boş olamaz.");
            if (string.IsNullOrWhiteSpace(varlikAdi)) 
                return Result.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Varlık adı boş olamaz.");
            if (string.IsNullOrWhiteSpace(varlikId)) 
                return Result.Fail(ErrorType.Validation, ErrorCodes.Common.ValidationError, "Varlık ID boş olamaz.");

            var entity = new DenetimKaydi
            {
                Eylem = eylem,
                VarlikAdi = varlikAdi,
                VarlikId = varlikId,
                DetayJson = detayJson,
                IpAdresi = ipAdresi,
                KullaniciId = kullaniciId,
                OlusturmaTarihi = System.DateTime.UtcNow
            };

            await _dal.AddAsync(entity, ct);
            await _unitOfWork.CommitAsync(ct);
            
            return Result.Success();
        }
    }
}
