using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLayer.Features.Kategoriler.DTOs;
using EntityLayer.Entities;

namespace BusinessLayer.Features.Kategoriler.Mappings
{
    public class KategoriProfile:Profile
    {
        public KategoriProfile()
        {
            CreateMap<KategoriDetailDto, Kategori>()
                        .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id))
                        .ForMember(dest => dest.UstKategoriId,
                    opt => opt.MapFrom(src => src.UstKategoriId))
                        .ForMember(dest => dest.Ad,
                    opt => opt.MapFrom(src => src.Ad))
                        .ForMember(dest => dest.SeoSlug,
                    opt => opt.MapFrom(src => src.SeoSlug))
                        .ForMember(dest => dest.AktifMi,
                    opt => opt.MapFrom(src => src.AktifMi))
                        .ForMember(dest => dest.OlusturmaTarihi,
                    opt => opt.MapFrom(src => src.OlusturmaTarihi))
                        .ForMember(dest => dest.GuncellemeTarihi,
                    opt => opt.MapFrom(src => src.GuncellemeTarihi))
                        .ForMember(dest => dest.SilindiMi,
                    opt => opt.MapFrom(src => src.SilindiMi))
                        .ForMember(dest => dest.UstKategori,
                    opt => opt.MapFrom(src => src.UstKategori));
        }
    }
}
