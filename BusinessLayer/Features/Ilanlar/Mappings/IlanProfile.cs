using AutoMapper;
using BusinessLayer.Features.Ilanlar.DTOs;
using DataAccessLayer.Projections;

namespace BusinessLayer.Features.Ilanlar.Mappings
{
    public class IlanProfile : Profile
    {
        public IlanProfile()
        {
            CreateMap<PendingListingProjection, PendingListingDto>();
        }
    }
}
