using AutoMapper;
using BusinessLayer.Features.Favoriler.DTOs;
using DataAccessLayer.Projections;

namespace BusinessLayer.Features.Favoriler.Mappings
{
    public class FavoriProfile : Profile
    {
        public FavoriProfile()
        {
            CreateMap<FavoriteListingProjection, FavoriteListingDto>();
        }
    }
}
