using EntityLayer.Entities;
using KKTCSatiyorum.Models;
using AutoMapper;

namespace KKTCSatiyorum.Mappings
{
    public class AccountProfile:Profile
    {
        public AccountProfile()
        {
            CreateMap<RegisterViewModel, UygulamaKullanicisi>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber,
                    opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.AdSoyad,
                    opt => opt.MapFrom(src => $"{src.Ad} {src.Soyad}"))
                .ForMember(dest => dest.AskidaMi,
                    opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.OlusturmaTarihi,
                    opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
