using BusinessLayer.Common.Constants;
using BusinessLayer.Common.Results;
using BusinessLayer.Features.DenetimKayitlari.DTOs;
using BusinessLayer.Features.DenetimKayitlari.Services;
using DataAccessLayer.Abstract;
using EntityLayer.DTOs.Admin;
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

        public DenetimKaydiManager(IDenetimKaydiDal dal)
        {
            _dal = dal;
        }

        public async Task<Result<PagedResult<DenetimKaydiListItemDto>>> GetPagedAsync(DenetimKaydiQuery query, CancellationToken ct)
        {
            // Validation
            if (query.Page < 1) query.Page = 1;
            if (query.PageSize < 1) query.PageSize = 25;
            if (query.PageSize > 100) query.PageSize = 100;

            var pagedProjection = await _dal.GetPagedAsync(query, ct);

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
            return Result.Success();
        }
    }
}
