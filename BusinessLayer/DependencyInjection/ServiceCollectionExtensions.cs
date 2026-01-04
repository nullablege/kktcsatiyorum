using AutoMapper;
using BusinessLayer.Features.Kategoriler.DTOs;
using BusinessLayer.Features.Kategoriler.Services;
using BusinessLayer.Features.Kategoriler.Validators;
using BusinessLayer.Features.KategoriAlanlari.DTOs;
using BusinessLayer.Features.KategoriAlanlari.Services;
using BusinessLayer.Features.KategoriAlanlari.Validators;
using BusinessLayer.Features.Ilanlar.DTOs;
using BusinessLayer.Features.Ilanlar.Services;
using BusinessLayer.Features.Ilanlar.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using BusinessLayer.Features.Kategoriler.Mappings;
using BusinessLayer.Features.DenetimKayitlari.Services;
using BusinessLayer.Features.DenetimKayitlari.Managers;
using BusinessLayer.Features.Favoriler.Services;
using BusinessLayer.Features.Bildirimler.Services;
using BusinessLayer.Features.Member.DTOs;
using BusinessLayer.Features.Member.Services;
using BusinessLayer.Features.Member.Validators;

namespace BusinessLayer.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(KategoriProfile).Assembly);

            // Kategori
            services.AddScoped<IKategoriService, KategoriService>();
            services.AddScoped<IKategoriSlugService, KategoriSlugService>();
            services.AddScoped<IValidator<CreateKategoriRequest>, CreateKategoriValidator>();
            services.AddScoped<IValidator<SoftDeleteKategoriRequest>, KategoriSoftDeleteValidator>();
            services.AddScoped<IValidator<UpdateKategoriRequest>, UpdateKategoriValidator>();

            // KategoriAlani
            services.AddScoped<IKategoriAlaniService, KategoriAlaniService>();
            services.AddScoped<IValidator<CreateKategoriAlaniRequest>, CreateKategoriAlaniValidator>();
            services.AddScoped<IValidator<UpdateKategoriAlaniRequest>, UpdateKategoriAlaniValidator>();

            // Ilan
            services.AddScoped<IIlanService, IlanService>();
            services.AddScoped<IValidator<CreateIlanRequest>, CreateIlanValidator>();
            services.AddScoped<IValidator<UpdateIlanRequest>, UpdateIlanValidator>();

            // DenetimKayitlari
            services.AddScoped<IDenetimKaydiService, DenetimKaydiManager>();

            // Favoriler
            services.AddScoped<IFavoriService, FavoriService>();

            // Bildirimler
            services.AddScoped<IBildirimService, BildirimService>();

            // Member

            // Member
            services.AddScoped<IMemberService, MemberService>();
            services.AddScoped<IValidator<UpdateProfileRequest>, UpdateProfileRequestValidator>();

            // Common
            services.AddScoped<BusinessLayer.Common.Abstractions.INotificationPublisher, BusinessLayer.Common.Services.NoOpNotificationPublisher>();

            return services;
        }
    }
}

