using AutoMapper;
using BusinessLayer.Features.Bildirimler.DTOs;
using DataAccessLayer.Projections;

namespace BusinessLayer.Features.Bildirimler.Mappings
{
    public class BildirimProfile : Profile
    {
        public BildirimProfile()
        {
            CreateMap<NotificationProjection, MyNotificationDto>();
        }
    }
}
