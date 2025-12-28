using AutoMapper;
using BusinessLayer.Features.KategoriAlanlari.DTOs;
using EntityLayer.Entities;

namespace BusinessLayer.Features.KategoriAlanlari.Mappings
{
    public class KategoriAlaniProfile : Profile
    {
        public KategoriAlaniProfile()
        {
            CreateMap<KategoriAlani, KategoriAlaniListItemDto>()
                .ForCtorParam("SecenekSayisi", opt => opt.MapFrom(src => src.Secenekler.Count));

            CreateMap<KategoriAlani, KategoriAlaniDetailDto>()
                .ForCtorParam("KategoriAdi", opt => opt.MapFrom(src => src.Kategori.Ad));

            CreateMap<KategoriAlaniSecenegi, KategoriAlaniSecenegiDto>();
        }
    }
}
