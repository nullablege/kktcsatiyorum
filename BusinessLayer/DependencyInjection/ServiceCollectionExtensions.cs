using AutoMapper;
using BusinessLayer.Features.Kategoriler.DTOs;
using BusinessLayer.Features.Kategoriler.Services;
using BusinessLayer.Features.Kategoriler.Validators;
using BusinessLayer.Features.KategoriAlanlari.DTOs;
using BusinessLayer.Features.KategoriAlanlari.Services;
using BusinessLayer.Features.KategoriAlanlari.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Features.Kategoriler.Mappings;

namespace BusinessLayer.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(KategoriProfile).Assembly);

            services.AddScoped<IKategoriService, KategoriService>();
            services.AddScoped<IKategoriSlugService, KategoriSlugService>();
            services.AddScoped<IValidator<CreateKategoriRequest>, CreateKategoriValidator>();
            services.AddScoped<IValidator<SoftDeleteKategoriRequest>, KategoriSoftDeleteValidator>();
            services.AddScoped<IValidator<UpdateKategoriRequest>, UpdateKategoriValidator>();

            services.AddScoped<IKategoriAlaniService, KategoriAlaniService>();
            services.AddScoped<IValidator<CreateKategoriAlaniRequest>, CreateKategoriAlaniValidator>();
            services.AddScoped<IValidator<UpdateKategoriAlaniRequest>, UpdateKategoriAlaniValidator>();

            return services;
        }
    }
}
